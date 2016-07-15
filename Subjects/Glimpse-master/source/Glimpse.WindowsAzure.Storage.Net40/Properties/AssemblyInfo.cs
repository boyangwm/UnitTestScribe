﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Glimpse.Core.Extensibility;

[assembly: ComVisible(false)]
[assembly: Guid("3b7a68a9-2d81-49c9-9838-c72698176b11")]

[assembly: AssemblyTitle("Glimpse Windows Azure Assembly")]
[assembly: AssemblyDescription("Windows Azure interfaces and types for Glimpse.")]// When you right-click the assembly file in Windows Explorer, this attribute appears as the Comments value on the Version tab of the file properties dialog box.
[assembly: AssemblyProduct("Glimpse.WindowsAzure")]
[assembly: AssemblyCopyright("© 2013 Nik Molnar & Anthony van der Hoorn")]
[assembly: AssemblyTrademark("Glimpse™")]

// Version is in major.minor.build format to support http://semver.org/
// Keep these three attributes in sync
[assembly: AssemblyVersion("1.8.0")]
[assembly: AssemblyFileVersion("1.8.0")]
[assembly: AssemblyInformationalVersion("1.8.0")] // Used to specify the NuGet version number at build time

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("Glimpse.Test.WindowsAzure")]
[assembly: NuGetPackage("Glimpse.WindowsAzure")]