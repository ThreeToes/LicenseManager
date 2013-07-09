using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using LicenseManagerLibrary.Licenses;
using Newtonsoft.Json;

namespace LicenseManagerLibrary.LicenseLoaders
{
    [Export(typeof (ILicenseLoaderSaver))]
    internal class JsonLicenseLoaderSaver : ILicenseLoaderSaver
    {
        [Import(typeof (ILicenseFactory))]
        public ILicenseFactory LicenseFactory { get; set; }

        #region ILicenseLoaderSaver Members

        public string Extension
        {
            get { return "json"; }
        }

        public ILicense LoadLicense(string path)
        {
            string json;
            json = GetJson(path);
            return LoadLicenseFromJson(json);
        }

        public void SaveLicense(ILicense license, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            string json = GetJson(license);
            using (var file = new StreamWriter(path))
            {
                file.Write(json);
            }
        }

        #endregion

        private string GetJson(string path)
        {
            string json;
            using (var reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }
            return json;
        }

        internal string GetJson(ILicense license)
        {
            return JsonConvert.SerializeObject(new LicenseSerializationContainer(license));
        }

        internal ILicense LoadLicenseFromJson(string json)
        {
            return JsonConvert.DeserializeObject<LicenseSerializationContainer>(json).GetLicense();
        }

        #region Serialization containers

        #region Nested type: FeatureSerializationContainer

        private class FeatureSerializationContainer
        {
            [JsonConstructor]
            public FeatureSerializationContainer()
            {
            }

            public FeatureSerializationContainer(ILicenseFeature feature)
            {
                Name = feature.Name;
                AccessLevel = feature.AccessLevel;
                Mandatory = feature.Mandatory;
                Enabled = feature.Enabled;
            }

            public bool Enabled { get; set; }

            public bool Mandatory { get; set; }

            public int AccessLevel { get; set; }

            public string Name { get; set; }

            public ILicenseFeature GetFeature()
            {
                return new SimpleFeature
                           {
                               AccessLevel = AccessLevel,
                               Enabled = Enabled,
                               Mandatory = Mandatory,
                               Name = Name
                           };
            }
        }

        #endregion

        #region Nested type: LicenseSerializationContainer

        private class LicenseSerializationContainer
        {
            [JsonConstructor]
            public LicenseSerializationContainer()
            {
            }

            public LicenseSerializationContainer(ILicense license)
            {
                AppName = license.ApplicationName;
                ExpiryDate = license.ExpiryDate;
                AccessLevel = license.LicenseAccessLevel;
                LicenseeName = license.LicenseeName;
                RootModule = new ModuleSerializationContainer(license.RootModule);
            }

            public ModuleSerializationContainer RootModule { get; set; }

            public string LicenseeName { get; set; }

            public int AccessLevel { get; set; }

            public DateTime ExpiryDate { get; set; }

            public string AppName { get; set; }

            public ILicense GetLicense()
            {
                var license = new SimpleLicense();

                license.ApplicationName = AppName;
                license.ExpiryDate = ExpiryDate;
                license.LicenseAccessLevel = AccessLevel;
                license.LicenseeName = LicenseeName;
                license.RootModule = RootModule.GetModule();

                return license;
            }
        }

        #endregion

        #region Nested type: ModuleSerializationContainer

        private class ModuleSerializationContainer
        {
            public ModuleSerializationContainer(ILicenseModule module)
            {
                Name = module.Name;
                Features = module.Features.Select(x => new FeatureSerializationContainer(x));
                Modules = module.SubModules.Select(x => new ModuleSerializationContainer(x));
            }

            [JsonConstructor]
            public ModuleSerializationContainer()
            {
            }

            public string Name { get; set; }
            public IEnumerable<FeatureSerializationContainer> Features { get; set; }
            public IEnumerable<ModuleSerializationContainer> Modules { get; set; }

            public ILicenseModule GetModule()
            {
                var module = new SimpleModule();

                module.Name = Name;
                module.Features = Features.Select(x => x.GetFeature());
                module.SubModules = Modules.Select(x => x.GetModule());

                return module;
            }
        }

        #endregion

        #endregion
    }
}