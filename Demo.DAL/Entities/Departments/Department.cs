using Demo.DAL.Entities.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Entities.Departments
{
    public class Department : ModelBase   //Department is a model Base
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Code { get; set; } = null!;
        public DateOnly CreationDate { get; set; }
        //Navegational Property //Department has a list of employees many
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();

    }
}
