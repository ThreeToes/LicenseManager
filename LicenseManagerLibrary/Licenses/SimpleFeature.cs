using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseManagerLibrary.Licenses
{
    class SimpleFeature : ILicenseFeature
    {
        public SimpleFeature(){}
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int AccessLevel { get; set; }
        public bool Mandatory { get; set; }
    }
}
