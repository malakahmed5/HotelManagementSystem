using HMS.Core.Entiites.RoomModuleEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Data.Configurations
{
    public class RoomConfigurations : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(r => r.RoomType)
                .HasConversion<string>();

            builder.Property(r => r.RoomStatus)
                .HasConversion<string>();

            builder.Property(r => r.Id)
                .UseIdentityColumn(100, 1);

            builder.Property(r => r.Description)
                .HasMaxLength(150);

            builder.Property(r => r.Id)
                .HasPrecision(18, 2);

            #region Relationships Configurations
            builder.HasMany(r => r.RoomImages)
                .WithOne()
                .HasForeignKey(img => img.RoomId)
                .OnDelete(DeleteBehavior.Cascade);


            #endregion
        }
    }
}
