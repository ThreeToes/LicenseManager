using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
	[Export(typeof(ILicenseLoaderSaver))]
	[Export(typeof(ISigningLoaderSaver))]
	class SignedFileLoaderSaver : ISigningLoaderSaver
	{
		[Import(typeof(ILicenseFactory))]
		public ILicenseFactory LicenseFactory { get; set; }

		private JsonLicenseLoaderSaver _inner;

		public SignedFileLoaderSaver()
		{
			_inner = new JsonLicenseLoaderSaver();
		}

		public string Extension
		{
			get { return ".lic"; }
		}

		public ILicense LoadLicense(string path)
		{
			throw new NotImplementedException();
		}

		public void SaveLicense(ILicense license, string path)
		{
			var tmpDir = Path.GetTempFileName();
			if(File.Exists(tmpDir)) {
				File.Delete(tmpDir);
			}
			Directory.CreateDirectory(tmpDir);
			var jsonLicensePath = Path.Combine(tmpDir, "license.json");
			_inner.SaveLicense(license,jsonLicensePath);
			var signaturePath = Path.Combine(tmpDir, "signature");
			var md5Hash = GetMd5Hash(jsonLicensePath);
			
			var pack = Package.Open(path);
			var licensePart = pack.CreatePart(new Uri("license"), "application/json", CompressionOption.Maximum);
			using(var stream = new StreamWriter(licensePart.GetStream()))
			{
				
			}
			pack.CreatePart(new Uri("signature"), "application/json", CompressionOption.Maximum);
			pack.Flush();
		}

		private string GetMd5Hash(string jsonLicensePath)
		{
			byte[] md5Hash;
			using (var md5 = MD5.Create())
			{
				using (var stream = File.OpenRead(jsonLicensePath))
				{
					md5Hash = md5.ComputeHash(stream);
				}
			}
			var result = new StringBuilder(md5Hash.Length * 2);

			for (int i = 0; i < md5Hash.Length; i++)
				result.Append(md5Hash[i].ToString("x2"));

			return result.ToString();
		}

		private static string RsaEncrypt(string privateKey, string dataToEncrypt)
		{
			string rsaPrivate = privateKey;
			var csp = new CspParameters();

			var provider = new RSACryptoServiceProvider(csp);

			provider.FromXmlString(rsaPrivate);

			var enc = new ASCIIEncoding();
			int numOfChars = enc.GetByteCount(dataToEncrypt);
			byte[] tempArray = enc.GetBytes(dataToEncrypt);
			byte[] result = provider.Encrypt(tempArray, true);
			string resultString = Convert.ToBase64String(result);
			return resultString;
		}

		public void SaveSignedLicense(string signingKey, ILicense license, string path)
		{
			throw new NotImplementedException();
		}
	}
}
