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
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.Property(A => A.Id).UseIdentityColumn(10, 10);
            builder.Property(A => A.LogLevel).HasColumnType("nvarchar(50)").IsRequired();
            builder.Property(A => A.Status).HasColumnType("bit").IsRequired();
            builder.Property(A => A.Message).HasColumnType("nvarchar(500)").IsRequired();
            builder.Property(A => A.Exception).HasColumnType("nvarchar(500)").IsRequired();
            builder.Property(A => A.LastModifiedOn).HasComputedColumnSql("GETDATE()");
            builder.Property(A => A.CreatedOn).HasDefaultValueSql("GETDATE()");
        }
    }
}
