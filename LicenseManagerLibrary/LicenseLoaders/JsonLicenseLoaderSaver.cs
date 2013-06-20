using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using LicenseManagerLibrary.Exceptions;
using LicenseManagerLibrary.Licenses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LicenseManagerLibrary.LicenseLoaders
{
    [Export(typeof(ILicenseLoaderSaver))]
    internal class JsonLicenseLoaderSaver : ILicenseLoaderSaver
    {

        [Import(typeof(ILicenseFactory))]
        public ILicenseFactory LicenseFactory { get; set; }

        public string Extension { get { return "json"; } }
        public ILicense LoadLicense(string path)
        {
            string json;
            using(var reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<LicenseSerializationContainer>(json).GetLicense();
        }

        public void SaveLicense(ILicense license, string path)
        {
            if(File.Exists(path)) {
                File.Delete(path);
            }
            var json = JsonConvert.SerializeObject(new LicenseSerializationContainer(license));
            using(var file = new StreamWriter(path)) {
                file.Write(json);
            }
        }

        #region Serialization containers
        private class LicenseSerializationContainer
        {
            public ModuleSerializationContainer RootModule { get; set; }

            [JsonConstructor]
            public LicenseSerializationContainer(){}

            public LicenseSerializationContainer(ILicense license)
            {
                AppName = license.ApplicationName;
                ExpiryDate = license.ExpiryDate;
                AccessLevel = license.LicenseAccessLevel;
                LicenseeName = license.LicenseeName;
                RootModule = new ModuleSerializationContainer(license.RootModule);
            }

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

        private class ModuleSerializationContainer
        {
            public string Name { get; set; }
            public IEnumerable<FeatureSerializationContainer> Features { get; set; }
            public IEnumerable<ModuleSerializationContainer> Modules { get; set; } 
            public ModuleSerializationContainer(ILicenseModule module)
            {
                Name = module.Name;
                Features = module.Features.Select(x => new FeatureSerializationContainer(x));
                Modules = module.SubModules.Select(x => new ModuleSerializationContainer(x));
            }

            [JsonConstructor]
            public ModuleSerializationContainer(){}

            public ILicenseModule GetModule()
            {
                var module = new SimpleModule();

                module.Name = Name;
                module.Features = Features.Select(x => x.GetFeature());
                module.SubModules = Modules.Select(x => x.GetModule());

                return module;
            }
        }

        private class FeatureSerializationContainer
        {
            [JsonConstructor]
            public FeatureSerializationContainer(){}

            public FeatureSerializationContainer(ILicenseFeature feature)
            {
                Name = feature.Name;
                AccessLevel = feature.AccessLevel;
                Mandatory = feature.Mandatory;
                Enabled = feature.Enabled;
            }

            public ILicenseFeature GetFeature()
            {
                return new SimpleFeature()
                           {
                               AccessLevel = AccessLevel,
                               Enabled = Enabled,
                               Mandatory = Mandatory,
                               Name = Name
                           };
            }

            public bool Enabled { get; set; }

            public bool Mandatory { get; set; }

            public int AccessLevel { get; set; }

            public string Name { get; set; }
        }
        #endregion
    }
}
