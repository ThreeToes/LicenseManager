using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary.LicenseLoaders
{
    internal interface ILicenseFactory
    {
        ILicense GetLicense();
        ILicenseModule GetModule();
        ILicenseFeature GetFeature();
        ILicense Clone(ILicense license);
    }
}