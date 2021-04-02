using System;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace SearchProject.Infrastructure
{
    public static class ElasticSearchExtension
    {
        private static readonly Options Options = new Options();

        public static void AddElasticsearchService(this IServiceCollection services, Action<Options> optionsAction)
        {
            optionsAction.Invoke(Options);

            var settings = new ConnectionSettings(Options.Uri)
                .DefaultIndex(Options.Index)
                .RequestTimeout(TimeSpan.FromMinutes(120));

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
        }
    }
    public class Options
    {
        public string Index { get; set; }
        public Uri Uri { get; set; }
    }
}
