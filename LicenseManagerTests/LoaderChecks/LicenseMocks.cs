using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerTests.LoaderChecks
{
    internal class Module : ILicenseModule
    {
        public string Name { get; set; }
        public IEnumerable<ILicenseModule> SubModules { get; set; }
        public IEnumerable<ILicenseFeature> Features { get; set; }

        public Module()
        {
            SubModules = new List<ILicenseModule>();
            Features = new List<ILicenseFeature>();
            Name = string.Empty;
        }
    }

    internal class Feature : ILicenseFeature
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int AccessLevel { get; set; }
        public bool Mandatory { get; set; }
    }

    internal class License : ILicense
    {
        private ILicenseModule root = new Module()
                                          {
                                              Features = new[]
                                                             {
                                                                 new Feature()
                                                                     {
                                                                         Name = "Modeller",
                                                                         AccessLevel = 1,
                                                                         Enabled = true,
                                                                         Mandatory = false
                                                                     }, 
                                                             },
                                                Name = "root",
                                                SubModules = new[]
                                                                 {
                                                                     new Module()
                                                                         {
                                                                             Name = "Submod1",
                                                                             Features = new[]
                                                                                            {
                                                                                                new Feature()
                                                                                                    {
                                                                                                        AccessLevel = 2,
                                                                                                        Enabled = false,
                                                                                                        Mandatory = true,
                                                                                                        Name = "JSON interface"
                                                                                                    }
                                                                                            }
                                                                         },
                                                                         new Module()
                                                                         {
                                                                             Name = "Submod2",
                                                                             Features = new[]
                                                                                            {
                                                                                                new Feature()
                                                                                                    {
                                                                                                        AccessLevel = 3,
                                                                                                        Enabled = true,
                                                                                                        Mandatory = false,
                                                                                                        Name = "XML interface"
                                                                                                    },
                                                                                                    new Feature()
                                                                                                    {
                                                                                                        AccessLevel = 0,
                                                                                                        Enabled = false,
                                                                                                        Mandatory = false,
                                                                                                        Name = "Super Secret Disabled feature"
                                                                                                    }
                                                                                            }
                                                                         }, 
                                                                 }
                                          };

        public string LicenseeName { get { return "Ada Lovelace"; } set { } }
        public string ApplicationName { get { return "License tests"; } set { } }
        public DateTime ExpiryDate { get { return DateTime.MaxValue; } set { } }

        public ILicenseModule RootModule { get { return root; } set { root = value; } }
        public int LicenseAccessLevel { get { return 1; } set{} }
        public bool IsValid(string featureName)
        {
            throw new NotImplementedException();
        }
    }
}
