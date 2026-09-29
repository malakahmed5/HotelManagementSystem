using HMS.Core.Entiites.AuthModule;
using HMS.Core.Entiites.BookingModule;
using HMS.Core.Entiites.RoomModuleEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMS.Infrastructure.Data.Configurations
{
    public class BookingConfigurations : BaseConfigurations<Guid,Booking> , IEntityTypeConfiguration<Booking>
    {
        public new void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.CheckInDate).IsRequired();
            builder.Property(b => b.CheckOutDate).IsRequired();
            builder.Property(b => b.TotalAmount).IsRequired();
            builder.Property(b => b.Status).IsRequired().HasConversion<string>();
            builder.Property(b => b.TotalAmount).HasPrecision(18, 2);

            #region Relationships Configurations
            builder.HasOne(b => b.GuestUser)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

        }
    }
}
