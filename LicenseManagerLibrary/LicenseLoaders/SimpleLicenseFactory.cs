using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
    [Export(typeof(ILicenseFactory))]
    class SimpleLicenseFactory : ILicenseFactory
    {
        public ILicense GetLicense()
        {
            return new SimpleLicense();
        }

        public ILicenseModule GetModule()
        {
            return new SimpleModule();
        }

        public ILicenseFeature GetFeature()
        {
            return new SimpleFeature();
        }

        public ILicense Clone(ILicense license)
        {
            var ret = new SimpleLicense();

            ret.LicenseAccessLevel = license.LicenseAccessLevel;
            ret.ApplicationName = license.ApplicationName;
            ret.ExpiryDate = license.ExpiryDate;
            ret.LicenseeName = license.LicenseeName;
            ret.RootModule = CloneModule(license.RootModule);

            return ret;
        }

        private SimpleModule CloneModule(ILicenseModule module)
        {
            var ret = new SimpleModule
                          {
                              Name = module.Name,
                              Features = new List<ILicenseFeature>(module.Features.Select(CloneFeature)),
                              SubModules = new List<ILicenseModule>(module.SubModules.Select(CloneModule))
                          };

            return ret;
        }

        private SimpleFeature CloneFeature(ILicenseFeature feature)
        {
            var ret = new SimpleFeature
                          {
                              AccessLevel = feature.AccessLevel,
                              Enabled = feature.Enabled,
                              Mandatory = feature.Enabled,
                              Name = feature.Name
                          };

            return ret;
        }
    }
}
