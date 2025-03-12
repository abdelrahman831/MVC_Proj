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
        IEnumerable<T> GetAll(bool AsNoTracking = true);
        IQueryable<T> GetAllQuarable();
        IEnumerable<T> GetAllEnumerble();

        T? GetById(int Id);
        void AddT(T entity);
        void UpdateT(T entity);
        void DeleteT(T entity);

    }
}
