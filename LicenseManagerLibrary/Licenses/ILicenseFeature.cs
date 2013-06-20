using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Licenses
{
    public interface ILicenseFeature
    {
        /// <summary>
        /// Name of the feature
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// If the feature is enabled
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Minimum access requirement
        /// </summary>
        int AccessLevel { get; set; }

        /// <summary>
        /// Whether this feature is enabled by default. 
        /// </summary>
        /// <remarks>
        /// Overrides AccessLevel and Enabled if set to true
        /// </remarks>
        bool Mandatory { get; set; }
    }
}
