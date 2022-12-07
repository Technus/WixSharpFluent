# WixSharpFluent

An extension, and fluent extension methods collection for:
https://github.com/oleg-shilo/wixsharp

## Typical use case

### Providing data from build scripts:

In your installer projects (not product projects) you might want to provide additonal information.
This can easily be done with Assembly Attributes:
 - WixSharp.Fluent.Attributes.*
 - Or the default Assembly Attributes

Which are read when calling respective methods from:
 - WixSharp.Fluent.Extensions.AssemblyAttributeExtensions
 - Or by the Setters for Project and in some cases when it's viable for Bundle (Can be easily checked by parameters list if it accepts a Assembly)

 It is also possible to read additonal information from the product assemblies.
 Just Provide The Assembly as parameter.

### CSPROJ

```XML
  <ItemGroup>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyInsideInstallerNameAttribute">
      <ProductNameVersion>$(ProductName) $(ProductShortVersion)</ProductNameVersion>
      <ReleaseCycle>$(ReleaseCycle)</ReleaseCycle><!-- Should be one of: alpha,beta,rc,rtm -->
      <!--Or instead of letting it compute from ProductNameVersion and ReleaseCycle define it directly:
      <ProductNameFull>$(ProductName) $(ProductVersion) - $(ReleaseCycleName)</ProductNameFull>
      -->
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyExecutableNameAttribute">
      <ExecutableName>$(Product)</ExecutableName>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyDefinesAttribute">
      <DefineConstants>$(DefineConstants)</DefineConstants>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyBundleUpgradeCodeAttribute">
      <UpgradeCode>**CONSTANT UNIQUE GUID HERE**</UpgradeCode>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyProjectUpgradeCodeAttribute">
      <UpgradeCode>**(ANOTHER) CONSTANT UNIQUE GUID HERE**</UpgradeCode>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyProgramFilesPathAttribute">
      <Path>$(Company)\$(Product)\</Path>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyStartMenuPathAttribute">
      <Path>$(Company)\$(Product)\</Path>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblySourcePathAttribute">
      <Path>$(PackagePath)\bin\$(Configuration)\</Path>
    </AssemblyAttribute>
    <AssemblyAttribute Include="WixSharp.Fluent.Attributes.AssemblyBootstrapperAttribute">
      ...Assembly Bootstrapper (Bundle) Settings
    </AssemblyAttribute>
    ...Other data...
  </ItemGroup>
```

### Project:

Assuming all data is provided from Assembly attributes of the Intaller project:

```C#
namespace Installer
{
  internal static class Product
  {
    public static readonly File mainExecutable = new File($@"{GetSourcePath()}\Product.exe").SetFireWallException();
    internal static readonly string mainFileName = System.IO.Path.GetFileName(mainExecutable.Name);
    internal static readonly string prettyName = GetExecutableName();

    internal static Project Create()
    {
      var product = new Project()
        .SetDefaults()
        //.SetPreserveTempFiles(true)
        .SetMajorUpgrade($"A Newer Version of {prettyName} is already installed.");

      //product.UI = WUI.WixUI_ProgressOnly;

      product.AddDirs(
          new InstallDir(InstallationFolderId.ToId(),GetInstallationPath(),
              new DirFiles($@"{GetSourcePath()}\*.*", f => !f.EndsWith(mainFileName) && !f.EndsWith(".pdb")),//'!f.EndsWith(exeFileName)' To not duplicate the file below
              mainExecutable//,new File(productFeature, $@"{GetSourcePath()}\pl-PL\Idefix.NoahDataExtractor.resources.dll")
              ),

          new Dir(GetStartMenuPath(),
              new ExeFileShortcut("ApplicationStartMenuShortcut".ToId(),
                      prettyName, $"[{InstallationFolderId}]{mainFileName}", arguments: "")
              {
                WorkingDirectory = $"[{InstallationFolderId}]",
                IconFile = product.GetIconPath(),
              }
            )
      );

      return product;
    }
  }
}
```

### Bundle:

```C#
namespace Installer
{
  public static class Program 
  {
    static void Main()
    {
      var project = Product.Create();
      new Bundle()
        .SetDefaults()
        .SetFromProject(project)
        .AddNetFxWeb48()//Redist
        .AddMsiProject(project)
        .Build();
    }
  }
}
```


