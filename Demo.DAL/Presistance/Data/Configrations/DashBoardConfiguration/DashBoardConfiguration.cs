using Demo.DAL.Entities.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Presistance.Data.Configrations.DashBoardConfiguration
{
    public class DashBoardConfiguration : IEntityTypeConfiguration<DashBoard>
    {
        public void Configure(EntityTypeBuilder<DashBoard> builder)
        {
            builder.Property(D => D.Id).UseIdentityColumn(10, 10);
            builder.Property(D => D.TotalUsers).HasColumnType("int").IsRequired();
            builder.Property(D => D.RegisteredUsersToday).HasColumnType("int").IsRequired();
            builder.Property(D => D.LoggedInUsers).HasColumnType("int").IsRequired();
            builder.Property(D => D.TotalDepartments).HasColumnType("int").IsRequired();
            builder.Property(D => D.TotalEmployees).HasColumnType("int").IsRequired();
            builder.Property(D => D.LastModifiedOn).HasComputedColumnSql("GETDATE()");
            builder.Property(D => D.CreatedOn).HasDefaultValueSql("GETDATE()");

        }
    }
}
