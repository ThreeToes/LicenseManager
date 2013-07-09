using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LicenseManagerLibrary;
using LicenseManagerLibrary.Licenses;
using NUnit.Framework;

namespace LicenseManagerTests.LoaderChecks
{
    /// <summary>
    /// This will automatically test any new data loaders we create
    /// </summary>
    [TestFixture]
    class SaverLoaderTests
    {
        private static ILicense GetLicense()
        {
            return new License();
        }

        private static LicenseManager GetLicenseManager()
        {
            return new LicenseManager();
        }

        [Test]
        public void SaveAndLoad()
        {
            var license = GetLicense();
            var manager = GetLicenseManager();
            var extensions = manager.SupportedExtensions;
            foreach (var extension in extensions)
            {
                var temp = GetTempLicenseFile(extension);
                manager.SaveLicense(license, temp);
                var loaded = manager.LoadLicense(temp);
                try
                {
                    LicenseTestTools.CheckLicense(license, loaded);
                }
                finally
                {
                    File.Delete(temp);
                }
            }
        }

        [Test]
        public void SaveAndLoadSigned()
        {
            var license = GetLicense();
            var manager = GetLicenseManager();
            var signedLoaders = manager.SignedLoaders;
            foreach (var loader in signedLoaders)
            {
                var extension = loader.Extension;
                var keys = loader.GenerateKeyPair();
                var temp = GetTempLicenseFile(extension);
                manager.SaveSignedLicense(keys.Item2,license, temp);
                var loaded = manager.LoadSignedLicense(keys.Item2,temp);
                try
                {
                    LicenseTestTools.CheckLicense(license, loaded);
                }
                finally
                {
                    File.Delete(temp);
                }
            }
        }

        private static string GetTempLicenseFile(string extension)
        {
            var temp = Path.GetTempFileName();
            File.Delete(temp);
            return temp + "." + extension;
        }
    }
}
