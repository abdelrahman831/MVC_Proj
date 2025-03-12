using Demo.DAL.Entities.Departments;
using Demo.DAL.Presistance.Data;
using Demo.DAL.Presistance.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Repositories.Departments
{
    //DataBase ==>Repository ==>Services ==>Controller
    public class DepartmentRepository : GenericRepository<Department>,IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
