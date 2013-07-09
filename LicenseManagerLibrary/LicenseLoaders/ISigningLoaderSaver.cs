using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
    public interface ISigningLoaderSaver : ILicenseLoaderSaver
	{
		void SaveSignedLicense(string signingKey, ILicense license, string path);
		ILicense LoadSignedLicense(string publicKey, string path);
	    Tuple<string, string> GenerateKeyPair();
	}
}
