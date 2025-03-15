using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Repositories.Generic
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        Task<IEnumerable<T>> GetAllAsync(bool AsNoTracking = true);
        IQueryable<T> GetAllQuarableAsync();
        Task<IEnumerable<T>> GetAllEnumerableAsync();

        Task<T?> GetByIdAsync(int Id);
        void AddTAsync(T entity);
        void UpdateTAsync(T entity);
        void DeleteTAsync(T entity);

    }
}
