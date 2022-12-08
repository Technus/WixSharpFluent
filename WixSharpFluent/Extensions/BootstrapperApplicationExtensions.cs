using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for Bootstrappers
    /// </summary>
    public static class BootstrapperApplicationExtensions
    {
        /// <summary>
        /// Sets OptionsUI and Repair to be enabled.
        /// Sets Version and FilresInUse to be enabled
        /// </summary>
        /// <typeparam name="ApplicationT"></typeparam>
        /// <param name="applicationBootstrapper"></param>
        /// <returns></returns>
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
