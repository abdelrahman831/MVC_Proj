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
        public IEnumerable<T> GetAll(bool AsNoTracking = true)
        {
            //IsDeleted ==>false
            if (AsNoTracking)
            {
                return _dbContext.Set<T>().Where(X=>!X.IsDeleted).AsNoTracking().ToList();  //Ditached
            }
            return _dbContext.Set<T>().Where(X => !X.IsDeleted).ToList();
            //UnChanged
        }

        public T? GetById(int Id)
        {
            //   return _dbContext.Ts.Local.FirstOrDefault(D => D.Id == Id); Old
            return _dbContext.Set<T>().Find(Id);  //Search localy in case "Found" ==>Return - In Case "NotFound" ==>will send to database to get it
        }
        public int AddT(T entity)
        {
            _dbContext.Set<T>().Add(entity);  //Saved localy
            return _dbContext.SaveChanges();       //Apply localy

        }
        public int UpdateT(T entity)
        {
            _dbContext.Set<T>().Update(entity);  //Modified
            return _dbContext.SaveChanges(); //unchanged
        }
        public int DeleteT(T entity)
        {
            //_dbContext.Set<T>().Remove(entity);  //Modified
            //return _dbContext.SaveChanges(); //unchanged
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
            return _dbContext.SaveChanges();
        }

        public IQueryable<T> GetAllQuarable()
        {
            return _dbContext.Set<T>();
        }

        public IEnumerable<T> GetAllEnumerble()
        {
            return _dbContext.Set<T>();
        }
    }
}
