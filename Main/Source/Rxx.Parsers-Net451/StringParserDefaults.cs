using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rxx.Parsers
{
  /// <summary>
  /// Provides default values for <see cref="StringParser{TResult}"/>.
  /// </summary>
  public static class StringParserDefaults
  {
    /// <summary>
    /// The default collection of whitespace characters returned by <see cref="StringParser{TResult}.InsignificantWhiteSpaceCharacters"/>.
    /// The collection contains a space, tab, new-line, and carriage return.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Object is immutable.")]
    public static readonly ICollection<char> InsignificantWhiteSpaceCharacters = new[]
		{
			' ', '\t', '\n', '\r'
		}
    .ToList()
    .AsReadOnly();
  }
}