using System;
using System.Collections.Generic;
using Dinokin.ScanlationTools.Functions;
using Dinokin.ScanlationTools.Interfaces;
using Dinokin.ScanlationTools.Rippers;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

namespace Dinokin.ScanlationTools
{
    public partial class App
    {
        private readonly IServiceProvider _services;

        public App()
        {
            var services = new ServiceCollection();
            
            services.AddHttpClient();
            services.AddTransient(typeof(HtmlWeb));
            services.AddTransient<IRipper, YoungAceUp>();
            services.AddTransient<IRipper, AlphaPolis>();
            services.AddTransient<IRipper, ComicRide>();
            services.AddTransient<IRipper, ComicBorder>();
            services.AddTransient<IRipper, SeigaNicoNico>();
            services.AddTransient<IFunction, Converter>();
            services.AddTransient<IFunction, PageJoiner>();
            services.AddTransient<IFunction, BorderRemover>();
            services.AddTransient<IFunction, Resizer>();

            _services = services.BuildServiceProvider();
        }

        public T GetService<T>() => _services.GetService<T>();

        public IEnumerable<T> GetServices<T>() => _services.GetServices<T>();
    }
}