using System.Collections.Generic;

namespace LicenseManagerLibrary.Licenses
{
    public interface ILicenseModule
    {
        string Name { get; set; }
        IEnumerable<ILicenseModule> SubModules { get; set; }
        IEnumerable<ILicenseFeature> Features { get; set; } 
    }
}