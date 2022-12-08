using System;
using System.Linq;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Installer Reinstall mode wrapper
    /// 
    /// https://documentation.help/Windows-Installer/reinstallmode.htm
    /// </summary>
    public sealed class ReinstallMode
    {
        /// <summary>
        /// Reinstall only if the file is missing.
        /// </summary>
        public static readonly ReinstallMode MissingFile = new ReinstallMode("p", true);

        /// <summary>
        /// Reinstall if the file is missing or is an older version.
        /// </summary>
        public static readonly ReinstallMode MissingOrOlderFile = new ReinstallMode("o", true);

        /// <summary>
        /// Reinstall if the file is missing, or is an equal or older version.
        /// </summary>
        public static readonly ReinstallMode MissingOrOlderOrEqualFile = new ReinstallMode("e", true);

        /// <summary>
        /// Reinstall if the file is missing or a different version is present.
        /// </summary>
        public static readonly ReinstallMode MissingOrDifferentFile = new ReinstallMode("d", true);

        /// <summary>
        /// Verify the checksum values, and reinstall the file if they are missing or corrupt. 
        /// This flag only repairs files that have msidbFileAttributesChecksum in the Attributes column of the File Table.
        /// </summary>
        public static readonly ReinstallMode MissingOrCorruptFile = new ReinstallMode("c", true);

        /// <summary>
        /// Force all files to be reinstalled, regardless of checksum or version.
        /// </summary>
        public static readonly ReinstallMode EveryFile = new ReinstallMode("a", true);

        /// <summary>
        /// Rewrite all required registry entries from the Registry Table that go to the:
        /// HKEY_CURRENT_USER or HKEY_USERS registry hive.
        /// </summary>
        public static readonly ReinstallMode RewriteUserRegistry = new ReinstallMode("u");

        /// <summary>
        /// Rewrite all required registry entries from the Registry Table that go to the:
        /// HKEY_LOCAL_MACHINE or HKEY_CLASSES_ROOT registry hive.
        /// Rewrite all information from the:
        /// Class Table, Verb Table, PublishComponent Table, ProgID Table, MIME Table, Icon Table, Extension Table, and AppID Table 
        /// regardless of machine or user assignment.
        /// Reinstall all qualified components.
        /// When reinstalling an application, this option runs the RegisterTypeLibraries and InstallODBC actions.
        /// </summary>
        public static readonly ReinstallMode RewriteMachineRegistry = new ReinstallMode("m");

        /// <summary>
        /// Reinstall all shortcuts and re-cache all icons overwriting any existing shortcuts and icons.
        /// </summary>
        public static readonly ReinstallMode RewriteShortcuts = new ReinstallMode("s");

        /// <summary>
        /// Use to run from the source package and re-cache the local package. 
        /// Do not use the v reinstall option code for the first installation of an application or feature.
        /// </summary>
        public static readonly ReinstallMode RunFromSourceReCacheLocal = new ReinstallMode("v");

        private readonly bool fileRelated;

        private string value;

        private ReinstallMode(string mode, bool isFileRelated = false)
        {
            value = mode;
            fileRelated = isFileRelated;
        }

        /// <summary>
        /// Joins both arumets, to single Reinstall Mode, also checks against multiple file behaviours.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">When both sides contain file mode</exception>
        public static ReinstallMode operator +(ReinstallMode a, ReinstallMode b)
        {
            if (a.fileRelated && b.fileRelated)
                throw new ArgumentException("Cannot specify multiple filed related behaviours");
            return new ReinstallMode(new string((a.value + b.value).ToCharArray().Distinct().ToArray()), a.fileRelated || b.fileRelated);
        }

        /// <summary>
        /// Returns the 'ToString'
        /// </summary>
        /// <param name="mode"></param>
        public static implicit operator string(ReinstallMode mode)
        {
            return mode.ToString();
        }

        /// <summary>
        /// Returns the string representing this reinstall Mode
        /// </summary>
        /// <returns></returns>
        public override string ToString() => value;
    }
}
