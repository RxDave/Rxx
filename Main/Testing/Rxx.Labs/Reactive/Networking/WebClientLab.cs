using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DaveSexton.Labs;
using Rxx.Labs.Properties;

namespace Rxx.Labs.Reactive.Networking
{
	[DisplayName("Web Client and Hosting (Updated)")]
	[Description("Making and serving web requests using ObservableWebClient and ObservableHttpListener.")]
	public sealed class WebClientLab : BaseConsoleLab
	{
		protected override void Main()
		{
			RunExperiments();
		}

		[Experiment("Download HTML Page")]
		[Description("Downloads the specified HTML page and finds all elements with id attributes.")]
		public void DownloadHtmlPageExperiment()
		{
			Uri address = UserInputUrl(Text.PromptFormat, Instructions.EnterAUrl);

			TraceLine(Text.Subscribing);
			TraceLine();

			using (var client = new WebClient())
			{
				var htmlTagsWithIds = client
					.DownloadStringObservable(address)
					.SelectMany(response =>
						Regex.Matches(
							response,
							@"\< (?<Tag> \w+? ) \s [^\>]*? id= (?<Q> [""']? ) (?<ID> .+? ) \k<Q> .*? \>",
								RegexOptions.IgnoreCase
							| RegexOptions.ExplicitCapture
							| RegexOptions.IgnorePatternWhitespace
							| RegexOptions.Singleline)
						.Cast<Match>())
					.Select(match => new
					{
						Tag = match.Groups["Tag"].Value,
						Id = match.Groups["ID"].Value
					});

				using (htmlTagsWithIds.Subscribe(ConsoleOutput))
				{
					TraceLine(Instructions.PressAnyKeyToCancel);

					WaitForKey();
				}
			}
		}

		[Description("Creates a local HTTP server, creates a local client, sends a request and then "
							 + "monitors download progress.")]
		public void DownloadDataWithProgressExperiment()
		{
			TraceLine(Instructions.PressAnyKeyToCancel);
			TraceLine();

			var address = new IPEndPoint(IPAddress.Loopback, 15005);

			using (ObservableHttpListener.Start(address).Subscribe(
				context =>
				{
					try
					{
						using (var response = context.Response)
						{
							var encoding = Encoding.UTF8;

							response.StatusCode = (int)HttpStatusCode.OK;
							response.ContentType = "text/plain";
							response.ContentEncoding = encoding;

							const string message = "This is a dummy response that will be repeated.";
							const int bytesPerBatch = 100 * 1000;
							const int batchCount = 5;

							int repeatCount = bytesPerBatch / message.Length;

							response.ContentLength64 = batchCount * repeatCount * message.Length;

							var stream = response.OutputStream;

							try
							{
								for (int i = 0; i < batchCount; i++)
								{
									var oneHundredKB = new string(message.Repeat(repeatCount).ToArray());
									var bytes = encoding.GetBytes(oneHundredKB);

									stream.Write(bytes, 0, bytes.Length);
									stream.Flush();

									System.Threading.Thread.Sleep(TimeSpan.FromSeconds(.5));
								}
							}
							catch (HttpListenerException)
							{
							}
						}
					}
					catch (InvalidOperationException)
					{
					}
				}))
			{
				var url = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + address.ToString());

				using (ObservableWebClient.DownloadDataWithProgress(url).SubscribeEither(
					ConsoleOutputOnNext<DownloadProgressChangedEventArgs>(
						Text.Progress,
						progress => string.Format(
							System.Globalization.CultureInfo.CurrentCulture,
							"{0,5:P0} = {1} of {2} byte(s)",
							progress.ProgressPercentage / 100f,
							progress.BytesReceived,
							progress.TotalBytesToReceive)),
					ConsoleOutputOnNext<byte[]>(Text.Result, bytes => bytes.Length + " byte(s)"),
					ConsoleOutputOnError(),
					ConsoleOutputOnCompleted()))
				{
					WaitForKey();
				}
			}
		}

		protected override async Task ExperimentExecutingAsync(IList<IExperiment> experiments, int index)
		{
			try
			{
				await base.ExperimentExecutingAsync(experiments, index);
			}
			catch (System.Reflection.TargetInvocationException ex)
			{
				// Catch the access denied error when the app is not running as an admin user account.
				if (ex.InnerException is HttpListenerException)
				{
					TraceError(ex.InnerException.Message);
				}
				else
				{
					throw;
				}
			}
		}
	}
}