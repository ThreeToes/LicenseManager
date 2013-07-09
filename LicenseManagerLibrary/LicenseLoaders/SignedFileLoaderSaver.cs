using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using LicenseManagerLibrary.Exceptions;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
	[Export(typeof(ILicenseLoaderSaver))]
	class SignedFileLoaderSaver : ISigningLoaderSaver
	{
	    private const string SignatureString = @"\signature.txt";
	    private const string LicenseString = @"\license.txt";

	    [Import(typeof(ILicenseFactory))]
		public ILicenseFactory LicenseFactory { get; set; }

		private JsonLicenseLoaderSaver _inner;

		public SignedFileLoaderSaver()
		{
			_inner = new JsonLicenseLoaderSaver();
		}

		public string Extension
		{
            //Because this uses file packers
			get { return "pck"; }
		}

		public ILicense LoadLicense(string path)
		{
            using (var pack = Package.Open(path))
            {

                //Get the signature
                var signature = pack.GetPart(PackUriHelper.CreatePartUri(new Uri(SignatureString, UriKind.Relative)));
                string sigString;
                using (var stream = new StreamReader(signature.GetStream()))
                {
                    sigString = stream.ReadToEnd();
                }

                //Get the license
                var license = pack.GetPart(PackUriHelper.CreatePartUri(new Uri(LicenseString, UriKind.Relative)));
                string licenseJson;
                using (var stream = new StreamReader(license.GetStream()))
                {
                    licenseJson = stream.ReadToEnd();
                }
                var enc = new ASCIIEncoding();
                var md5 = enc.GetString(GetMd5Hash(licenseJson));
                if (md5 != sigString)
                {
                    throw new InvalidLicenseException("License hash doesn't match!");
                }

                return _inner.LoadLicenseFromJson(licenseJson);
            }
		}

        private bool VerifySignature(byte[] signature, byte[] hash, string xmlKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlKey);
            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("MD5");
            return (rsaDeformatter.VerifySignature(hash, signature));
        }

		public void SaveLicense(ILicense license, string path)
		{
            if(File.Exists(path)) {
                File.Delete(path);
            }
            using (var pack = Package.Open(path))
            {
                var licenseString = _inner.GetJson(license);
                var md5 = GetMd5Hash(licenseString);

                var licensePart = pack.CreatePart(
                    PackUriHelper.CreatePartUri(new Uri(LicenseString, UriKind.Relative)),
                    System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
                var signaturePart =
                    pack.CreatePart(PackUriHelper.CreatePartUri(new Uri(SignatureString, UriKind.Relative)),
                                    System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);

                using (var stream = new StreamWriter(licensePart.GetStream()))
                {
                    stream.Write(licenseString);
                }

                using (var stream = new StreamWriter(signaturePart.GetStream()))
                {
                    var enc = new ASCIIEncoding();
                    stream.Write(enc.GetString(md5));
                }

                pack.Flush();
            }
		}

		private byte[] GetMd5Hash(string json)
		{
			byte[] md5Hash;
		    var enc = new ASCIIEncoding();
			using (var md5 = MD5.Create()) {
				md5Hash = md5.ComputeHash(enc.GetBytes(json));
			}
			return md5Hash;
		}


        public void SaveSignedLicense(string signingKey, ILicense license, string path)
		{
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using(var pack = Package.Open(path))
            {
                var licenseString = _inner.GetJson(license);
                var md5 = GetSignature(GetMd5Hash(licenseString), signingKey);

                var licensePart = pack.CreatePart(
                    PackUriHelper.CreatePartUri(new Uri(LicenseString, UriKind.Relative)),
                    System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);
                var signaturePart = pack.CreatePart(
                    PackUriHelper.CreatePartUri(new Uri(SignatureString, UriKind.Relative)),
                    System.Net.Mime.MediaTypeNames.Text.Plain, CompressionOption.Maximum);

                using (var stream = new StreamWriter(licensePart.GetStream()))
                {
                    stream.Write(licenseString);
                }

                var enc = new ASCIIEncoding();
                using (var stream = new StreamWriter(signaturePart.GetStream()))
                {
                    stream.Write(enc.GetString(md5));
                }
            }
		}

	    private byte[] GetSignature(byte[] hashValue, string signingKey)
	    {
            //The value to hold the signed value.

	        //Generate a public/private key pair.
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(signingKey);

            //Create an RSAPKCS1SignatureFormatter object and pass it the 
            //RSACryptoServiceProvider to transfer the private key.
            var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);

            //Set the hash algorithm to SHA1.
            rsaFormatter.SetHashAlgorithm("MD5");

            //Create a signature for HashValue and assign it to 
            //SignedHashValue.
	        return rsaFormatter.CreateSignature(hashValue);
	    }

	    public ILicense LoadSignedLicense(string publicKey, string path)
	    {
            using (var pack = Package.Open(path))
            {

                //Get the signature
                var signature = pack.GetPart(PackUriHelper.CreatePartUri(new Uri(SignatureString, UriKind.Relative)));
                byte[] sigString;
                using (var stream = new StreamReader(signature.GetStream()))
                {
                    var enc = new ASCIIEncoding();
                    sigString = enc.GetBytes(stream.ReadToEnd());
                }

                //Get the license
                var license = pack.GetPart(PackUriHelper.CreatePartUri(new Uri(LicenseString, UriKind.Relative)));
                string licenseJson;
                using (var stream = new StreamReader(license.GetStream()))
                {
                    licenseJson = stream.ReadToEnd();
                }

                var md5 = GetMd5Hash(licenseJson);
                if (VerifySignature(sigString,md5, publicKey))
                {
                    throw new InvalidLicenseException("License hash doesn't match!");
                }

                return _inner.LoadLicenseFromJson(licenseJson);
            }
	    }

	    public Tuple<string, string> GenerateKeyPair()
	    {
            // Create a new key pair on target CSP
            var cspParams = new CspParameters();
            cspParams.ProviderType = 1; // PROV_RSA_FULL 
            //cspParams.ProviderName; // CSP name
            cspParams.Flags = CspProviderFlags.UseArchivableKey;
            cspParams.KeyNumber = (int)KeyNumber.Exchange;
            var rsaProvider = new RSACryptoServiceProvider(cspParams);

            // Export public key
            var publicKey = rsaProvider.ToXmlString(false);

            // Export private/public key pair 
            var privateKey = rsaProvider.ToXmlString(true);
            return new Tuple<string, string>(publicKey, privateKey);
	    }
	}
}
