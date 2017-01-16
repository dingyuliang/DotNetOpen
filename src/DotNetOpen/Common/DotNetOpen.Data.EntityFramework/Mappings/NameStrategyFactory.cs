using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DotNetOpen.Data.EntityFramework.Mappings.NameStrategy;

namespace DotNetOpen.Data.EntityFramework.Mappings
{
    public static class NameStrategyFactory
    {
        #region Const
        public const string EntityTablePrefix = "Tbl";
        public const string XrefTablePrefix = "Xref";
        public const string FoeignKeySuffix = "Id";
        public const string RelationDomainSuffix = "Relation";
        #endregion

        #region Name Strategy
        /// <summary>
        /// Basic Strategy
        /// </summary>
        static readonly INameStrategy _AddUnderscoresBetweenWordsThenToLowerNameStrategy = new CompositeNameStrategy
        {
            Strategies = new INameStrategy[]{
                new AddUnderscoresBetweenWordsNameStrategy(),
                new ToLowerNameStrategy()
            }
        };

        /// <summary>
        /// Table Strategy
        /// </summary>
        static readonly INameStrategy _EntityNameStrategy = new CompositeNameStrategy
        {
            Strategies = new List<INameStrategy>{
                new AddPrefixNameStrategy(EntityTablePrefix),
                _AddUnderscoresBetweenWordsThenToLowerNameStrategy
            }
        };
        
        /// <summary>
        /// Table Strategy
        /// </summary>
        static readonly INameStrategy _XrefNameStrategy =new CompositeNameStrategy
        {
            Strategies = new List<INameStrategy>{
                new AddPrefixNameStrategy(XrefTablePrefix),
                new RemoveSuffixNameStrategy(RelationDomainSuffix),
                _AddUnderscoresBetweenWordsThenToLowerNameStrategy
            }
        };

        /// <summary>
        /// Column Strategy
        /// </summary>
        static readonly INameStrategy _ForeignKeyNoPrefixNameStrategy = new CompositeNameStrategy
        {
            Strategies = new INameStrategy[]{
                new AddSuffixNameStrategy(FoeignKeySuffix),
                _AddUnderscoresBetweenWordsThenToLowerNameStrategy
            }
        };

        /// <summary>
        /// Column Strategy, for Foreign Key
        /// </summary>
        static readonly INameStrategy _AddFoeignKeySuffixNameStrategy = new AddSuffixNameStrategy(FoeignKeySuffix);
        #endregion

        #region Column Name Strategy
        static readonly IColumnNameStrategy _AddTypeNameAsPrefixColumnNameStrategy = new AddTypeNameAsPrefixColumnNameStrategy();

        public static readonly IColumnNameStrategy UnderscoreColumnStrategory = new ColumnNameStrategyAdaptor(_AddUnderscoresBetweenWordsThenToLowerNameStrategy);

        public static readonly IColumnNameStrategy PrimitiveColumnNameStrategy = new CompositeColumnNameStrategy
        {
            ColumnNameStrategy = _AddTypeNameAsPrefixColumnNameStrategy,     
            Strategies = new List<INameStrategy>{
                _AddUnderscoresBetweenWordsThenToLowerNameStrategy
            }
        };

        public static readonly IColumnNameStrategy ForeignKeyColumnNameStrategy = new CompositeColumnNameStrategy
        {
            ColumnNameStrategy = _AddTypeNameAsPrefixColumnNameStrategy, 
            Strategies = new List<INameStrategy> {
                _AddFoeignKeySuffixNameStrategy,
                _AddUnderscoresBetweenWordsThenToLowerNameStrategy
            }
        };

        /// <summary>
        /// get column name strategry
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IColumnNameStrategy GetColumnNameStrategy(ColumnNameStrategyType type)
        {
            switch (type)
            {
                case ColumnNameStrategyType.Regular:
                    return UnderscoreColumnStrategory;
                case ColumnNameStrategyType.Primitive:
                default:
                    return PrimitiveColumnNameStrategy;
            }
        }
        #endregion

        #region GetTableNameStrategy
        public static ITableNameStrategy XrefTableNameStrategy = new TableNameStrategyAdaptor(_XrefNameStrategy);
        public static ITableNameStrategy EntityTableNameStrategy = new TableNameStrategyAdaptor(_EntityNameStrategy);
        /// <summary>
        /// get table name strategy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ITableNameStrategy GetTableNameStrategy(TableNameStrategyType type)
        {
            switch (type)
            {
                case TableNameStrategyType.XrefTable:
                    return XrefTableNameStrategy;
                case TableNameStrategyType.EntityTable:
                default:
                    return EntityTableNameStrategy;
            }
        }
        #endregion
    }

    #region TableName Strategy and Column Name Strategy Adaptor
    internal class TableNameStrategyAdaptor : ITableNameStrategy
    {
        internal TableNameStrategyAdaptor(INameStrategy nameStrategy)
        {
            NameStrategy = nameStrategy;
        }

        internal INameStrategy NameStrategy { get; private set; }

        public string ToTable(Type entityType)
        {
            return NameStrategy.ToName(entityType.Name);
        }
    }

    internal class ColumnNameStrategyAdaptor : IColumnNameStrategy
    {
        internal ColumnNameStrategyAdaptor(INameStrategy nameStrategy)
        {
            NameStrategy = nameStrategy;
        }

        internal INameStrategy NameStrategy { get; private set; }

        public string ToColumn(Type entityType, PropertyInfo propertyInfo)
        {
            return NameStrategy.ToName(propertyInfo.Name);
        }
    }
    #endregion

    #region Table Name Strategy Type
    public enum TableNameStrategyType
    {
        /// <summary>
        /// Regular Entity Table
        /// </summary>
        EntityTable,
        /// <summary>
        /// Xref Table
        /// </summary>
        XrefTable,
    }
    #endregion

    #region Column Name Strategy Type
    public enum ColumnNameStrategyType
    {
        /// <summary>
        /// include table name prefix, and under score
        /// </summary>
        Primitive,
        /// <summary>
        /// include under score
        /// </summary>
        Regular
    }
    #endregion
}
