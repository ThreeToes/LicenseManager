using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LicenseManagerLibrary.Licenses
{
    internal class SimpleModule : ILicenseModule
    {
        public string Name { get; set; }
        public IEnumerable<ILicenseModule> SubModules { get; set; }
        public IEnumerable<ILicenseFeature> Features { get; set; }

        public SimpleModule()
        {
            Name = string.Empty;
            SubModules = new List<ILicenseModule>();
            Features = new List<ILicenseFeature>();
        }
    }
}
