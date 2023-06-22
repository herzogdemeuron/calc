using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calc.Core.DirectusAPI.StorageDrivers;
using Calc.Core.Objects;
using Calc.Core.DirectusAPI;
using Polly;
using GraphQL.Client.Http;
using Autodesk.Revit.UI;

namespace Calc.ConnectorRevit
{
    public class DirectusManager
    {
        private readonly Directus Directus;
        private readonly BuildupStorageDriver buildupStorageDriver;
        private readonly ForestStorageDriver forestStorageDriver;
        private readonly MappingStorageDriver mappingStorageDriver;
        
        public List<Buildup> AllBuildups { get; set; }
        public List<Forest> AllForests { get; set; }
        public List<Mapping> AllMappings { get; set; }


        public DirectusManager() 
        {
            Directus = new Directus();
            buildupStorageDriver = new BuildupStorageDriver(Directus);
            forestStorageDriver = new ForestStorageDriver(Directus);
            mappingStorageDriver = new MappingStorageDriver(Directus);
        }

        public async Task Initiate()
        {
            try
            {
                var policy = Policy.Handle<GraphQLHttpRequestException>()
                    .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(5),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        TaskDialog.Show("Error", "Cannot communicate to directus, please try again.");
                        Console.WriteLine($"An error occurred and the operation is being retried for the {retryCount} time.");

                        if (retryCount == 4)
                        {
                            Environment.Exit(1);
                        }
                    });

                AllBuildups = await policy.ExecuteAsync(() => GetAllBuildups());
                AllForests = await policy.ExecuteAsync(() => GetAllForests());
                AllMappings = await policy.ExecuteAsync(() => GetAllMappings());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<List<Buildup>> GetAllBuildups()
        {
            return await buildupStorageDriver.GetAllBuildupsFromDirectus();
        }
        public async Task<List<Forest>> GetAllForests()
        {
            return await forestStorageDriver.GetAllForestsFromDirectus();
        }
        public async Task<List<Mapping>> GetAllMappings()
        {
            return await mappingStorageDriver.GetAllMappingsFromDirectus();
        }
    }
}
