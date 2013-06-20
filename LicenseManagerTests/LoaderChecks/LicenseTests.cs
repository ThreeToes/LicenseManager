using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LicenseManagerLibrary;
using NUnit.Framework;

namespace LicenseManagerTests.LoaderChecks
{
    /// <summary>
    /// This will assume that only one type of license loader is used at a time. Can't see any reason I'd need more
    /// than one implementation of the ILicense interfaces at any point, but doing it this way will make it
    /// easier to replace without breaking functionality
    /// </summary>
    [TestFixture]
    class LicenseTests
    {
        private static LicenseManager GetManager()
        {
            return new LicenseManager();
        }

        [Test]
        public void TestValidAccessLevel()
        {
            var manager = GetManager();
            var temp = LicenseTestTools.SaveLicenseToLoad(manager);
            try
            {
                var license = manager.LoadLicense(temp);
                Assert.IsTrue(license.IsValid("Modeller"));
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Test]
        public void TestInvalidAccessLevel()
        {
            var manager = GetManager();
            var temp = LicenseTestTools.SaveLicenseToLoad(manager);
            try
            {
                var license = manager.LoadLicense(temp);
                Assert.IsFalse(license.IsValid("XML interface"));
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Test]
        public void TestMandatory()
        {
            var manager = GetManager();
            var temp = LicenseTestTools.SaveLicenseToLoad(manager);
            try
            {
                var license = manager.LoadLicense(temp);
                license.LicenseAccessLevel = 0;
                Assert.IsTrue(license.IsValid("JSON interface"));
            }
            finally
            {
                File.Delete(temp);
            }
        }

        [Test]
        public void TestExpired()
        {
            var manager = GetManager();
            var temp = LicenseTestTools.SaveLicenseToLoad(manager);
            try
            {
                var license = manager.LoadLicense(temp);
                license.ExpiryDate = DateTime.MinValue;
                Assert.IsFalse(license.IsValid("JSON interface"));
            }
            finally
            {
                File.Delete(temp);
            }
        }



        [Test]
        public void TestDisabled()
        {
            var manager = GetManager();
            var temp = LicenseTestTools.SaveLicenseToLoad(manager);
            try
            {
                var license = manager.LoadLicense(temp);
                Assert.IsFalse(license.IsValid("Super Secret Disabled feature"));
            }
            finally
            {
                File.Delete(temp);
            }
        }
    }
}
