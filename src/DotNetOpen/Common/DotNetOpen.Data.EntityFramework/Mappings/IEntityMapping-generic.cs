using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework.Mappings
{
    public interface IEntityMapping
    {
        Type EntityType { get; }
    }

    public interface IEntityMapping<TEntity>: IEntityMapping
        where TEntity : class
    {
        void Map(EntityTypeConfiguration<TEntity> typeBuilder);
    }
}
