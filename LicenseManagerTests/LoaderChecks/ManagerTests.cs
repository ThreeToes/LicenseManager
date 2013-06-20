using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LicenseManagerLibrary;
using NUnit.Framework;

namespace LicenseManagerTests.LoaderChecks
{
    [TestFixture]
    class ManagerTests
    {
        private static LicenseManager GetManager()
        {
            return new LicenseManager();
        }

        [Test]
        public void TestSaveAndLoad()
        {
            var manager = GetManager();
            var license = new License();
            var temp = Path.GetTempFileName();
            File.Delete(temp);
            temp = temp + "." + manager.SupportedExtensions.First();
            try
            {
                manager.SaveLicense(license, temp);
                var loaded = manager.LoadLicense(temp);
                LicenseTestTools.CheckLicense(license,loaded);
            }finally
            {
                if(File.Exists(temp)) File.Delete(temp);
            }
        }
    }
}
