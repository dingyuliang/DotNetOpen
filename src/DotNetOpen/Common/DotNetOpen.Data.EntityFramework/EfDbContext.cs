using Castle.Windsor;
using DotNetOpen.Data.EntityFramework.Mappings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework
{
    public class EfDbContext : DbContext
    {
        #region Const
        const string METHOD_ENTITYMAPPING_MAP = "Map";
        const string METHOD_MODELBUILDER_ENTITY = "Entity";
        #endregion

        #region Ctor
        public EfDbContext(IEntityMapping[] mappings)
            : base()
        {
            Mappings = mappings;
        }
        #endregion

        #region Properties
        public IEnumerable<IEntityMapping> Mappings { get; private set; }
        #endregion

        #region Override
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var entityMappingGenericType = typeof(IEntityMapping<>);
            var entityTypeGenericType = typeof(EntityTypeConfiguration<>);
            var genericEntityMethodInfo = typeof(DbModelBuilder).GetMethods().Where(a => a.IsGenericMethod && a.Name == METHOD_MODELBUILDER_ENTITY && !a.GetParameters().Any()).Single();
            foreach (var mapping in Mappings)
            {
                if (entityMappingGenericType.MakeGenericType(mapping.EntityType).IsAssignableFrom(mapping.GetType()))
                {
                    var genericEntityMethodWithoutParams = genericEntityMethodInfo.MakeGenericMethod(mapping.EntityType);
                    var genericEntity = genericEntityMethodWithoutParams.Invoke(modelBuilder, null);
                    var registerMethod = mapping.GetType().GetMethod(METHOD_ENTITYMAPPING_MAP, new Type[] { entityTypeGenericType.MakeGenericType(mapping.EntityType) });
                    registerMethod.Invoke(mapping, new object[] { genericEntity });
                }
            }
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
