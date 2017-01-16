using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace DotNetOpen.Data.EntityFramework.Mappings
{ 
    internal class ConfigurableEntityMapping<T> : IEntityMapping<T>
        where T : class
    {
        #region Ctor
        public ConfigurableEntityMapping(Action<EntityTypeConfiguration<T>> typeBuilderConfigure)
        {

            TypeBuilderConfigureT = typeBuilderConfigure;
        }
        #endregion

        #region Properties
        public Action<EntityTypeConfiguration<T>> TypeBuilderConfigureT { get; private set; }
        #endregion

        #region Implement IEntityMapping<T>
        public Type EntityType
        {
            get
            {
                return typeof(T);
            }
        }
        
        public void Map(EntityTypeConfiguration<T> typeBuilder)
        {
            if (TypeBuilderConfigureT != null)
            {
                TypeBuilderConfigureT(typeBuilder);
            }
        }
        #endregion
    }
}
