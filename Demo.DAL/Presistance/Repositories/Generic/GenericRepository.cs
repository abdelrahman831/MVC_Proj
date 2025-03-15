using Demo.DAL.Entities;
using Demo.DAL.Entities.Departments;
using Demo.DAL.Presistance.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Repositories.Generic
{
    public class GenericRepository<T> :IGenericRepository<T> where T : ModelBase
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)  //Ask Clr to create object from Applicationdbcontext
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool AsNoTracking = true)
        {
            //IsDeleted ==>false
            if (AsNoTracking)
            {
                return  await _dbContext.Set<T>().Where(X=>!X.IsDeleted).AsNoTracking().ToListAsync();  //Ditached
            }
            return await _dbContext.Set<T>().Where(X => !X.IsDeleted).ToListAsync();
            //UnChanged
        }

        public async Task<T?> GetByIdAsync(int Id)
        {
            //   return _dbContext.Ts.Local.FirstOrDefault(D => D.Id == Id); Old
            return await _dbContext.Set<T>().FindAsync(Id);  //Search localy in case "Found" ==>Return - In Case "NotFound" ==>will send to database to get it
        }
        public void AddTAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);  //Saved localy

        }
        public void UpdateTAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);  //Modified
//unchanged
        }
        public void DeleteTAsync(T entity)
        {
            //_dbContext.Set<T>().Remove(entity);  //Modified
            //return _dbContext.SaveChanges(); //unchanged
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);

        }

        public IQueryable<T> GetAllQuarableAsync()
        {
            return  _dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllEnumerableAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
    }
}
