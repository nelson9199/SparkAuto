using Microsoft.Extensions.Localization;
using SparkAuto.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SparkAuto.Services
{
    public class CommonLocalizationService
    {
        private readonly IStringLocalizer localizer;
        public CommonLocalizationService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(ModelBindingMessages).GetTypeInfo().Assembly.FullName);
            localizer = factory.Create(nameof(ModelBindingMessages), assemblyName.Name);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
