using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Licenses
{
    public interface ILicense
    {
        /// <summary>
        /// Name of the person/organisation this is licensed to
        /// </summary>
        string LicenseeName { get; set; }

        /// <summary>
        /// Name of the application
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// License expiry date
        /// </summary>
        DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Root module
        /// </summary>
        ILicenseModule RootModule { get; set; }
        
        /// <summary>
        /// Access level of this license
        /// </summary>
        int LicenseAccessLevel { get; set; }

        /// <summary>
        /// Whether a feature is valid
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <returns>Whether the feature is valid</returns>
        bool IsValid(string featureName);
    }
}
