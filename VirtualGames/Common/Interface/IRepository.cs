using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualGames.Data;

namespace VirtualGames.Common.Interface
{
    public interface IRepository<T> where T : BaseDataItem
    {
        Task<IEnumerable<T>> ReadAsync(string query);

        Task<T> ReadByIdAsync(string id);

        Task UpdateAsync(T item);

        Task<T> CreateAsync(T item);

        Task<T> DeleteAsync(T item);

        void SetPartitionKey(string key);
    }
}