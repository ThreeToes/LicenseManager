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
    static class LicenseTestTools
    {
        public static void CheckModule(ILicenseModule module, ILicenseModule corresponding)
        {
            Assert.AreEqual(module.Name, corresponding.Name);
            var featureCount = module.Features.Count();
            Assert.AreEqual(featureCount, corresponding.Features.Count());
            for(var i =0; i < featureCount; i ++)
            {
                CheckFeature(module.Features.ElementAt(i), corresponding.Features.ElementAt(i));
            }
            var moduleCount = module.SubModules.Count();
            Assert.AreEqual(moduleCount, corresponding.SubModules.Count());
            for(var i = 0; i < moduleCount; i++)
            {
                CheckModule(module.SubModules.ElementAt(i), corresponding.SubModules.ElementAt(i));
            }
        }

        public static void CheckFeature(ILicenseFeature feature, ILicenseFeature corresponding)
        {
            Assert.AreEqual(feature.AccessLevel, corresponding.AccessLevel);
            Assert.AreEqual(feature.Enabled, corresponding.Enabled);
            Assert.AreEqual(feature.Mandatory, corresponding.Mandatory);
            Assert.AreEqual(feature.Name, corresponding.Name);
        }

        public static void CheckLicense(ILicense license, ILicense loaded)
        {
            Assert.AreEqual(license.LicenseAccessLevel, loaded.LicenseAccessLevel);
            Assert.AreEqual(license.ApplicationName, loaded.ApplicationName);
            Assert.AreEqual(license.ExpiryDate, loaded.ExpiryDate);
            CheckModule(license.RootModule, loaded.RootModule);
        }

        /// <summary>
        /// The problem with the license mock is that I want to test whatever
        /// my library is giving back. I'll save my mock and load it again
        /// to get around this
        /// </summary>
        /// <returns></returns>
        public static string SaveLicenseToLoad(LicenseManager manager)
        {
            string temp = Path.GetTempFileName();
            File.Delete(temp);
            var extension = manager.Loaders.First().Extension;
            temp = temp + "." + extension;
            var license = new License();
            manager.SaveLicense(license,temp);
            return temp;
        }
    }
}
