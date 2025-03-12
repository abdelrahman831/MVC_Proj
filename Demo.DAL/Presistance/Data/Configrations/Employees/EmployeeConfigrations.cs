using Demo.DAL.Entities.Common.Enums;
using Demo.DAL.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Data.Configrations.Employees
{
    public class EmployeeConfigrations : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.Name).HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(e=>e.Address).HasColumnType("nvarchar(100)");
            builder.Property(e => e.Salary).HasColumnType("decimal(8,2)");
            builder.Property(D => D.LastModifiedOn).HasComputedColumnSql("GETDATE()");
            builder.Property(D => D.CreatedOn).HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.Gender).HasConversion(    //Database Template
                (gender)=>gender.ToString(),      //String in Database
                 (gender)=>(Gender)Enum.Parse(typeof(Gender), gender)   //gender

                 );

            builder.Property(e => e.EmployeeType).HasConversion(
                (type)=>type.ToString(),
                (type)=> (EmployeeType)Enum.Parse(typeof(EmployeeType), type)
                 
                );
           
        }
    }
}
