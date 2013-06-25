using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
	interface ISigningLoaderSaver : ILicenseLoaderSaver
	{
		void SaveSignedLicense(string signingKey, ILicense license, string path);
		void LoadSignedLicense(string publicKey, string path);
	}
}
