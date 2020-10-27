using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualGames.Data;

namespace VirtualGames.Common.Interface
{
    public interface IRepository<T> where T : BaseDataItem
    {
        Task<IEnumerable<T>> ReadAsync(string query, string partitionKey = "Default");

        Task UpdateAsync(T item, string partitionKey = "Default");

        Task<T> CreateAsync(T item, string partitionKey = "Default");
    }
}