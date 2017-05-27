using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("VSIXProject1")]
[assembly: AssemblyDescription("")]

[assembly: ProvideCodeBase(AssemblyName = "System.Reactive.Core", CodeBase = @"$PackageFolder$\System.Reactive.Core.dll")]
[assembly: ProvideCodeBase(AssemblyName = "System.Reactive.Interfaces", CodeBase = @"$PackageFolder$\System.Reactive.Interfaces.dll")]
[assembly: ProvideCodeBase(AssemblyName = "System.Reactive.Linq", CodeBase = @"$PackageFolder$\System.Reactive.Linq.dll")]
[assembly: ProvideCodeBase(AssemblyName = "System.Reactive.PlatformServices", CodeBase = @"$PackageFolder$\System.Reactive.PlatformServices.dll")]
[assembly: ProvideCodeBase(AssemblyName = "System.Reactive.Windows.Threading", CodeBase = @"$PackageFolder$\System.Reactive.Windows.Threading.dll")]
[assembly: ProvideCodeBase(AssemblyName = "ReactiveUI", CodeBase = @"$PackageFolder$\ReactiveUI.dll")]
[assembly: ProvideCodeBase(AssemblyName = "Splat", CodeBase = @"$PackageFolder$\Splat.dll")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

