using System.Reflection;
using System.Runtime.CompilerServices;

// The name of the company.

[assembly: AssemblyCompany(@"Stas Sultanov")]

// The configuration of the assembly.

#if DEBUG

[assembly: AssemblyConfiguration(@"Debug")]

#else

[assembly: AssemblyConfiguration(@"Retail")]

#endif

// The copyright of the assembly.

[assembly: AssemblyCopyright(@"Copyright © 2015 Stas Sultanov")]

// The culture of the assembly.

[assembly: AssemblyCulture(@"")]

// The description of the assembly.

[assembly: AssemblyDescription(@"Contains types that extends .NET Framework.")]

// The version number of the file.
// Is managed by the build process.

[assembly: AssemblyFileVersion(@"255.255.255.255")]

// The product name information.

[assembly: AssemblyProduct(@".NET Framework Extensions")]

// The title of the assembly.

[assembly: AssemblyTitle(@"SXN.Core")]

// The trademark of the assembly.

[assembly: AssemblyTrademark(@"")]

// The version number of the assembly.
// Is managed by the build process.

[assembly: AssemblyVersion(@"255.255.255.255")]

#if DEBUG

// The tests assembly.

[assembly: InternalsVisibleTo(@"SXN.Core.Tests")]

#endif