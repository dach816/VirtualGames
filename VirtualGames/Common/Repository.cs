using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using VirtualGames.Common.Interface;
using VirtualGames.Data;

namespace VirtualGames.Common
{
    public class Repository<T> : IRepository<T>
        where T : BaseDataItem
    {
        private readonly Container _container;
        private string _partitionKey;

        public Repository(CosmosClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            _container = database.GetContainer(typeof(T).Name);
            _partitionKey = "Default";
        }

        public async Task<IEnumerable<T>> ReadAsync(string query)
        {
            var results = new List<T>();
            using var resultSet = _container.GetItemQueryIterator<T>(
                string.IsNullOrEmpty(query) ? null : new QueryDefinition(query),
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(_partitionKey)
                });
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<T> ReadByIdAsync(string id)
        {
            return await _container.ReadItemAsync<T>(
                partitionKey: new PartitionKey(_partitionKey),
                id: id);
        }

        public async Task UpdateAsync(T item)
        {
            await _container.ReplaceItemAsync(
                partitionKey: new PartitionKey(_partitionKey),
                id: item.Id,
                item: item);
        }

        public async Task<T> CreateAsync(T item)
        {
            return await _container.CreateItemAsync(item, new PartitionKey(_partitionKey));
        }

        public async Task<T> DeleteAsync(T item)
        {
            return await _container.DeleteItemAsync<T>(item.Id, new PartitionKey(_partitionKey));
        }

        public void SetPartitionKey(string key)
        {
            _partitionKey = key;
        }
    }
}