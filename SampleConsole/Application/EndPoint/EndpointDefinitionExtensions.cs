using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SampleConsole.Application.EndPoint
{
    public static class EndpointDefinitionExtensions
    {
        /// <summary>
        /// IServiceCollection登録
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scanMakers"></param>
        public static void AddEndpointDefinitions(this IServiceCollection services, params Type[] scanMakers)
        {
            var endpointDefinitions = new List<IEndpointDefinition>();

            foreach (var scanMaker in scanMakers)
            {
                endpointDefinitions.AddRange(
                    scanMaker.Assembly.ExportedTypes
                        .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) 
                        && !x.IsInterface && !x.IsAbstract)
                        .Select(Activator.CreateInstance).Cast<IEndpointDefinition>()
                    );
            }

            foreach (var endpointDefinition in endpointDefinitions)
            {
                endpointDefinition.DefineServices(services);
            }

            services.AddSingleton(endpointDefinitions as IReadOnlyCollection<IEndpointDefinition>);
        }

        /// <summary>
        /// WebApplicationに上記のServicesを登録
        /// </summary>
        /// <param name="app"></param>
        public static void UseEndpointDefinitions(this WebApplication app)
        {
            var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

            foreach (var endpointDefinition in definitions)
            {
                endpointDefinition.DefineEndpoints(app);
            }
        }

    }
}
