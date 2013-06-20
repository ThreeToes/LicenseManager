﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using LicenseManagerLibrary.Exceptions;
using LicenseManagerLibrary.LicenseLoaders;
using LicenseManagerLibrary.Licenses;

namespace LicenseManagerLibrary
{
    public class LicenseManager
    {
        private CompositionContainer _container;

        [ImportMany(typeof(ILicenseLoaderSaver))]
        public IEnumerable<ILicenseLoaderSaver> Loaders { get; set; }

        public IEnumerable<string> SupportedExtensions
        {
            get { return Loaders.Select(x => x.Extension); }
        } 

        public LicenseManager()
        {
            var catalog = new AssemblyCatalog(GetType().Assembly);
            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }

        public LicenseManager(IEnumerable<string> allowedExtensions) : this()
        {
            Loaders = new List<ILicenseLoaderSaver>(Loaders.Where(x => allowedExtensions.Contains(x.Extension)));
        }

        public ILicense LoadLicense(string path)
        {
            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
            {
                throw new UnsupportedFileTypeException("Cannot choose file strategy without an extension");
            }
            if (extension.First() == '.')
            {
                extension = extension.Substring(1);
            }
            var loader = Loaders.FirstOrDefault(x => x.Extension == extension);
            if(loader == null)
            {
                throw new UnsupportedFileTypeException("Unsupported filetype " + extension);
            }
            return loader.LoadLicense(path);
        }

        public void SaveLicense(ILicense license, string path)
        {
            var extension = Path.GetExtension(path);
            if(string.IsNullOrEmpty(extension))
            {
                throw new UnsupportedFileTypeException("Cannot choose file strategy without an extension");
            }
            if(extension.First() == '.')
            {
                extension = extension.Substring(1);
            }
            var saver = Loaders.FirstOrDefault(x => x.Extension == extension);
            if (saver == null)
            {
                throw new UnsupportedFileTypeException("Unsupported filetype " + extension);
            }
            saver.SaveLicense(license,path);
            
        }
    }
}
