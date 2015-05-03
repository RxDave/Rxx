#pragma warning disable 0436
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Rxx.UnitTests, PublicKey="
  + "00240000048000009400000006020000002400005253413100040000010001008b9eb24ee1e273"
  + "f6c54c5e13b17f8b5ba836de94c86a05b0b833a68e56604e10f0c8747186523b32e57dc603fd9d"
  + "367c3e90e019b9d44f72ffe21e2dcdfe8e60814625e88c858f078536604f6ed2bcccc0c72d2245"
  + "70d32d01b218c80508e15e07351630c242d76f4e16722ee5ede147479e9fc9160999d3d33f24c9"
  + "022beba1")]

[assembly: CLSCompliant(true)]

[assembly: NeutralResourcesLanguage("en-US")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyConfiguration(AssemblyConstants.Configuration)]

[assembly: AssemblyCompany("Dave Sexton")]		// Replaces $author$ token in NuGet package
[assembly: AssemblyProduct("Rxx for Rx 2.x")]
[assembly: AssemblyCopyright("Copyright © 2011-2015 Rxx")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion(AssemblyConstants.Version + ".0")]
[assembly: AssemblyFileVersion(AssemblyConstants.Version + ".0")]

[assembly: SuppressMessage("Microsoft.Usage", "CA2243:AttributeStringLiteralsShouldParseCorrectly", Justification = "Required for NuGet semantic versioning.")]
[assembly: AssemblyInformationalVersion(AssemblyConstants.Version + AssemblyConstants.PrereleaseVersion)]

[SuppressMessage("Microsoft.Design", "CA1050:DeclareTypesInNamespaces", Justification = "Referenced by assembly-level attributes only.")]
internal static class AssemblyConstants
{
  public const string Version = "2.0.0";

  /// <summary>
  /// Semantic version for the assembly, indicating a prerelease package in NuGet.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The specified name can be arbitrary, but its mere presence indicates a prerelease package.
  /// To indicate a release package instead, use an empty string.
  /// </para>
  /// <para>
  /// If specified, the value must include a preceding hyphen; e.g., "-alpha", "-beta", "-rc".
  /// </para>
  /// </remarks>
  /// <seealso href="http://docs.nuget.org/docs/reference/versioning#Really_brief_introduction_to_SemVer">
  /// NuGet Semantic Versioning
  /// </seealso>
  public const string PrereleaseVersion = "";

#if DEBUG
  public const string Configuration = "Debug";
#else
  public const string Configuration = "Release";
#endif
}