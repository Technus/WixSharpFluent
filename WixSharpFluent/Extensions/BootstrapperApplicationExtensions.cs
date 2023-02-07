using System;
using WixSharp.Bootstrapper;
using WixSharp.Fluent.XML;

namespace WixSharp.Fluent.Extensions
{
    /// <summary>
    /// Extensions for Bootstrappers
    /// </summary>
    public static class BootstrapperApplicationExtensions
    {
        /// <summary>
        /// Wix Extension: Bal Extended
        /// </summary>
        public static WixExtension BalExt = new WixExtension(AppDomain.CurrentDomain.BaseDirectory.PathCombine("WixBalExtensionExt.dll"), "bal", "http://schemas.microsoft.com/wix/BalExtension");

        /// <summary>
        /// Holder for the Simple Bal extension
        /// </summary>
        public static WixExtension Bal = WixExtension.Bal;


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
