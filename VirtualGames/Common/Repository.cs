using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using VirtualGames.Common.Interface;

namespace VirtualGames.Common
{
    public class Repository<T> : IRepository<T>
    {
        private readonly Container _container;

        public Repository(CosmosClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _container = database.GetContainer(typeof(T).Name);
        }

        public async Task<IEnumerable<T>> ReadAsync(string partitionKey = "Default")
        {
            var results = new List<T>();
            using var resultSet = _container.GetItemQueryIterator<T>(
                queryDefinition: null,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKey)
                });
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }
    }
}