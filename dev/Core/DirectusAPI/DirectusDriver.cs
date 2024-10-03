using Calc.Core.DirectusAPI.StorageDrivers;
using GraphQL;
using System;
using System.Threading.Tasks;

namespace Calc.Core.DirectusAPI
{
    /// <summary>
    /// A static class that provides interfaces to interact with the Directus.
    /// </summary>
    internal static class DirectusDriver
    {
        internal static Directus DirectusInstance { get; set; }

        internal static async Task<TDriver> GetMany<TDriver, T>(IDriverGetMany<T> storageDriver)
        {
            var request = CreateRequest(storageDriver.QueryGetMany);
            var response = await DirectusInstance.GraphQlQueryWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            return response.Data;
        }

        internal static async Task<TDriver> GetSingle<TDriver, T>(IDriverGetSingle<T> storageDriver) where TDriver : IDriverGetSingle<T>
        {
            var request = CreateRequest(storageDriver.QueryGetSingle);
            var response = await DirectusInstance.GraphQlQueryWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            // Update the driver with the retrieved item
            storageDriver.GotItem = response.Data.GotItem;
            return response.Data;
        }

        internal static async Task<TDriver> GetManySystem<TDriver,T>(IDriverGetManySystem<T> storageDriver) where TDriver : IDriverGetManySystem<T>
        {
            var request = CreateRequest(storageDriver.QueryGetManySystem);
            var response = await DirectusInstance.GraphQlSysQueryWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            return response.Data;
        }

        internal static async Task<TDriver> CreateSingle<TDriver,T>(IDriverCreateSingle<T> storageDriver) where TDriver : IDriverCreateSingle<T>
        {
            CheckDriverPropertyIsNotNull(storageDriver, "SendItem");
            var request = CreateRequest(storageDriver.QueryCreateSingle, storageDriver.GetVariables());
            var response = await DirectusInstance.GraphQlMutationWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            // Update the driver with the created item
            storageDriver.CreatedItem = response.Data.CreatedItem;
            return response.Data;
        }

        internal static async Task<TDriver> CreateMany<TDriver,T>(IDriverCreateMany<T> storageDriver) where TDriver : IDriverCreateMany<T>
        {
            CheckDriverPropertyIsNotNull(storageDriver, "SendItems");
            var request = CreateRequest(storageDriver.QueryCreateMany, storageDriver.GetVariables());
            var response = await DirectusInstance.GraphQlMutationWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            return response.Data;
        }

        internal static async Task<TDriver> UpdateSingle<TDriver,T>(IDriverUpdateSingle<T> storageDriver) where TDriver : IDriverUpdateSingle<T>
        {
            CheckDriverPropertyIsNotNull(storageDriver, "SendItem");
            var request = CreateRequest(storageDriver.QueryUpdateSingle, storageDriver.GetVariables());
            var response = await DirectusInstance.GraphQlMutationWithRetry<TDriver>(request);
            if (response.Errors != null)
            {
                throw new Exception($"Error: {response.Errors[0].Message}");
            }
            return response.Data;
        }

        private static GraphQLRequest CreateRequest(string query, object variables = null)
        {
            return new GraphQLRequest { Query = query, Variables = variables };
        }

        private static void CheckDriverPropertyIsNotNull<TDriver>(TDriver storageDriver, string propertyName)
        {
            if (storageDriver == null)
            {
                throw new Exception($"StorageDriver {typeof(TDriver).Name} is null.");
            }
            var property = typeof(TDriver).GetProperty(propertyName) ?? throw new Exception($"Property {propertyName} does not exist on driver {typeof(TDriver).Name}.");
            _ = property.GetValue(storageDriver) ?? throw new Exception($"Property {propertyName} on driver {typeof(TDriver).Name} is null.");
        }
    }
}
