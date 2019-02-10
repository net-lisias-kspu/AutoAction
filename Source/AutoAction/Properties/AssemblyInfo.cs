﻿using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("AutoAction")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("AutoAction")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("eed6fc53-c8f7-4dca-82a9-df0102408adc")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyFileVersion(AutoAction.Version.Number)]
[assembly: AssemblyVersion(AutoAction.Version.Number)]

// Use KSPAssembly to allow other DLLs to make this DLL a dependency in a 
// non-hacky way in KSP.  Format is (AssemblyProduct, major, minor), and it 
// does not appear to have a hard requirement to match the assembly version. 
[assembly: KSPAssembly("AutoAction", AutoAction.Version.major, AutoAction.Version.minor)]

[assembly: KSPAssemblyDependency("KSPe", 2, 1)]
