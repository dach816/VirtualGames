using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtualGames.Common.Interface
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> ReadAsync(string partitionKey = "Default");
    }
}