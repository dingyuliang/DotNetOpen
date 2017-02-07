using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.Data
{
    public static  class DataExtensions
    {
        /// <summary>
        /// run Query for filter, search, order, paging.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDataInfo"></typeparam>
        /// <param name="entities"></param>
        /// <param name="listInput"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> RunQuery<TEntity, TDataInfo>(this IQueryable<TEntity> entities, ListInput<TDataInfo> listInput)
            where TEntity : class
        {
            // filter
            foreach (var option in listInput.FilterOptions)
                if (!option.Field.StartsWith(listInput.IgnoredPrefix))
                    entities = entities.FilteBy(option.Field, option.Value);

            // search
            var searchFields = listInput.SearchFields.Where(a => !a.StartsWith(listInput.IgnoredPrefix)).ToList();
            if (searchFields.Any())
            {
                if (listInput.SearchOperator == SearchOperator.OR)
                {
                    entities = entities.LikeAnyOr(searchFields, new string[] { listInput.SearchText });
                }
                else
                {
                    entities = entities.LikeAnyAnd(searchFields, new string[] { listInput.SearchText });
                }
            }

            // order
            var sortOptions = listInput.SortOptions.Where(a => !string.IsNullOrEmpty(a.Field) && !a.Field.StartsWith(listInput.IgnoredPrefix)).ToList();
            var sortIndex = 0;
            foreach (var sortOption in sortOptions)
            {
                entities = entities.Sort(sortOption.Field, sortOption.Direction == SortDirection.ASC, sortIndex++ > 0);
            }

            // paging
            entities = entities.Paging(listInput.PageNumber, listInput.PageSize);

            return entities;
        }
        /// <summary>
        /// run Query for filter, search, order, paging, generate output model.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDataInfo"></typeparam>
        /// <param name="entities"></param>
        /// <param name="listInput"></param>
        /// <returns></returns>
        public static ListOutput<TEntity, TDataInfo> RunOutput<TEntity, TDataInfo>(this IQueryable<TEntity> entities, ListInput<TDataInfo> listInput)
            where TEntity : class
        {
            var result = new ListOutput<TEntity, TDataInfo>();

            // set TotalCount
            result.TotalCount = entities.Count();

            /*filter*/
            foreach (var option in listInput.FilterOptions)
                if (!option.Field.StartsWith(listInput.IgnoredPrefix))
                    entities = entities.FilteBy(option.Field, option.Value);

            /*search*/
            var searchFields = listInput.SearchFields.Where(a => !a.StartsWith(listInput.IgnoredPrefix)).ToList();
            if (searchFields.Any() && !string.IsNullOrEmpty(listInput.SearchText))
            {
                if (listInput.SearchOperator == SearchOperator.OR)
                {
                    entities = entities.LikeAnyOr(searchFields, new string[] { listInput.SearchText });
                }
                else
                {
                    entities = entities.LikeAnyAnd(searchFields, new string[] { listInput.SearchText });
                }
            }
            // set MatchCount
            result.MatchCount = entities.Count();

            /*order*/
            var sortOptions = listInput.SortOptions.Where(a => !string.IsNullOrEmpty(a.Field) && !a.Field.StartsWith(listInput.IgnoredPrefix)).ToList();
            var sortIndex = 0;
            foreach (var sortOption in sortOptions)
            {
                entities = entities.Sort(sortOption.Field, sortOption.Direction == SortDirection.ASC, sortIndex++ > 0);
            }

            /*paging*/
            entities = entities.Paging(listInput.PageNumber, listInput.PageSize);

            // set Items.
            result.Items = entities;

            result.Data = listInput.Data;

            return result;
        }
        /// <summary>
        /// Select Items to another entity type.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDataInfo"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="output"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static ListOutput<TResult, TDataInfo> Select<TEntity, TDataInfo, TResult>(this ListOutput<TEntity, TDataInfo> output, Expression<Func<TEntity, TResult>> selector)
        => new ListOutput<TResult, TDataInfo>()
        {
            Data = output.Data,
            MatchCount = output.MatchCount,
            TotalCount = output.TotalCount,
            Items = output.Items.AsQueryable().Select(selector)
        };
        /// <summary>
        /// Make Items to List
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDataInfo"></typeparam>
        /// <param name="output"></param>
        /// <returns></returns>
        public static ListOutput<TEntity, TDataInfo> ToList<TEntity, TDataInfo>(this ListOutput<TEntity, TDataInfo> output)
        {
            output.Items = output.Items.ToList();
            return output;
        }
        public static ListOutput<TEntity, TDataInfo> OrderBy<TEntity, TDataInfo, TKey>(this ListOutput<TEntity, TDataInfo> output, Expression<Func<TEntity, TKey>> keySelector)
        {
            output.Items = output.Items.AsQueryable().OrderBy<TEntity, TKey>(keySelector);
            return output;
        }
        public static ListOutput<TEntity, TDataInfo> OrderByDescending<TEntity, TDataInfo, TKey>(this ListOutput<TEntity, TDataInfo> output, Expression<Func<TEntity, TKey>> keySelector)
        {
            output.Items = output.Items.AsQueryable().OrderByDescending<TEntity, TKey>(keySelector);
            return output;
        }
        public static ListOutput<TEntity, TDataInfo> ThenBy<TEntity, TDataInfo, TKey>(this ListOutput<TEntity, TDataInfo> output, Expression<Func<TEntity, TKey>> keySelector)
        {
            if (output.Items is IOrderedQueryable<TEntity>)
                output.Items = (output.Items as IOrderedQueryable<TEntity>).ThenBy<TEntity, TKey>(keySelector);
            return output;
        }

        public static ListOutput<TEntity, TDataInfo> ThenByDescending<TEntity, TDataInfo, TKey>(this ListOutput<TEntity, TDataInfo> output, Expression<Func<TEntity, TKey>> keySelector)
        {
            if (output.Items is IOrderedQueryable<TEntity>)
                output.Items = (output.Items as IOrderedQueryable<TEntity>).ThenByDescending<TEntity, TKey>(keySelector);
            return output;
        }
    }
}
