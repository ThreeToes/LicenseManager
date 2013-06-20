using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Licenses
{
    internal class SimpleLicense : ILicense
    {
        public string LicenseeName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public ILicenseModule RootModule { get; set; }
        public int LicenseAccessLevel { get; set; }
        
        public bool IsValid(string featureName) {
            return CheckModule(RootModule, featureName);
        }

        public SimpleLicense()
        {
            RootModule = new SimpleModule();
        }

        protected bool CheckModule(ILicenseModule module, string feature) {
            if (DateTime.Now > ExpiryDate) return false;
            if(module.Features.Any(x => x.Name == feature && ((x.AccessLevel <= LicenseAccessLevel && x.Enabled) || x.Mandatory))) {
                return true;
            }
            return module.SubModules.Any(x => CheckModule(x, feature));
        }
    }
}
