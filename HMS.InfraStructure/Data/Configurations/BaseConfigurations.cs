using HMS.Core.Entiites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Data.Configurations
{
    public abstract class BaseConfigurations<Tkey, TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<Tkey>
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
