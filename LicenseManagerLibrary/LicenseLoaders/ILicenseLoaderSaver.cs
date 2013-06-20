using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
    public interface ILicenseLoaderSaver
    {
        /// <summary>
        /// Extension the loader can read
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Load a license
        /// </summary>
        /// <param name="path">Path to the license</param>
        /// <returns>The loaded license</returns>
        ILicense LoadLicense(string path);

        /// <summary>
        /// Save a license
        /// </summary>
        /// <param name="license">The license to save</param>
        /// <param name="path">The path to save to</param>
        void SaveLicense(ILicense license, string path);
    }
}
