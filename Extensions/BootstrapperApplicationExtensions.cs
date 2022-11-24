using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.Extensions
{
    public static class BootstrapperApplicationExtensions
    {
        public static ApplicationT SetDefaults<ApplicationT>(this ApplicationT applicationBootstrapper) where ApplicationT : WixStandardBootstrapperApplication
        {
            applicationBootstrapper.SuppressOptionsUI = false;
            applicationBootstrapper.SuppressRepair = false;
            applicationBootstrapper.ShowVersion = true;
            applicationBootstrapper.ShowFilesInUse = true;
            return applicationBootstrapper;
        }
    }
}
