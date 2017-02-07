﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetOpen.Common
{
    public static class EnumerableExtension
    {
        #region Static
        public static readonly char[] DefaultSeperators = { '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', ' ', ';', ':', '"', '\'', '{', '}', '[', ']', '\\', '|', '?', '/', ',', '<', '.', '>' };
        private static readonly MethodInfo QueryableOrderByMethod = null;
        private static readonly MethodInfo QueryableOrderByDescendingMethod = null;
        private static readonly MethodInfo QueryableThenByMethod = null;
        private static readonly MethodInfo QueryableThenByDescendingMethod = null;
        #endregion

        #region Ctor
        static EnumerableExtension()
        {
            var allMethods = typeof(System.Linq.Queryable).GetMethods();
            QueryableOrderByMethod = allMethods.Where(a => a.Name.Equals("OrderBy") && a.GetParameters().Count() == 2).FirstOrDefault();
            QueryableOrderByDescendingMethod = allMethods.Where(a => a.Name.Equals("OrderByDescending") && a.GetParameters().Count() == 2).FirstOrDefault();
            QueryableThenByMethod = allMethods.Where(a => a.Name.Equals("ThenBy") && a.GetParameters().Count() == 2).FirstOrDefault();
            QueryableThenByDescendingMethod = allMethods.Where(a => a.Name.Equals("ThenByDescending") && a.GetParameters().Count() == 2).FirstOrDefault();
        }
        #endregion

        #region IQueryable Extensions

        #region Paging
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="page">index from 1</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Paging<TEntity>(this IQueryable<TEntity> entities, int page, int pageSize)
           where TEntity : class
        {
            return entities.Skip((page - 1) * pageSize).Take(pageSize);
        }
        #endregion

        #region Sort
        public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> entities, string propertyName, bool isAscending = true, bool useThenBy = false)
           where TEntity : class
            => Check.NotNull(entities, nameof(entities)).Sort(ExpressionHelper.BuildPropertyExpression(typeof(TEntity), propertyName) as LambdaExpression, isAscending, useThenBy);

        public static IOrderedQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> entities, LambdaExpression propertyExpression, bool isAscending = true, bool useThenBy = false)
           where TEntity : class
        {
            var propertyType = ExpressionHelper.GetPropertyType(propertyExpression);
            var genericParameters = new Type[] { typeof(TEntity), propertyType };
            var delegateType = typeof(Func<,>).MakeGenericType(genericParameters);
            var sortLambdaExpression = Expression.Lambda(delegateType, propertyExpression.Body, propertyExpression.Parameters);
            var method = default(MethodInfo);
            if (useThenBy)
            {
                if (isAscending)
                    method = QueryableThenByMethod.MakeGenericMethod(genericParameters);
                else
                    method = QueryableThenByDescendingMethod.MakeGenericMethod(genericParameters);
            }
            else
            {
                if (isAscending)
                    method = QueryableOrderByMethod.MakeGenericMethod(genericParameters);
                else
                    method = QueryableOrderByDescendingMethod.MakeGenericMethod(genericParameters);
            }
            return method.Invoke(null, new object[] { entities, sortLambdaExpression }) as IOrderedQueryable<TEntity>; ;
        }

        public static IQueryable<TEntity> Sort<TEntity, Tkey>(this IQueryable<TEntity> entities, Expression<Func<TEntity, Tkey>> keySelector, bool isAscending, bool useThenBy)
           where TEntity : class
        {

            if (useThenBy && (entities as IOrderedQueryable<TEntity>) != null)
            {
                var orderedQueryable = entities as IOrderedQueryable<TEntity>;
                return isAscending
                            ? orderedQueryable.ThenBy(keySelector)
                            : orderedQueryable.ThenByDescending(keySelector);
            }
            else
            {
                return isAscending
                            ? entities.OrderBy(keySelector)
                            : entities.OrderByDescending(keySelector);
            }
        }
        #endregion

        #region Filter
        public static IQueryable<TEntity> FilteBy<TEntity>(this IQueryable<TEntity> entities,
                                                           string filterField,
                                                           object filteValue)
            => Check.NotNull(entities, nameof(entities)).FilteBy(ExpressionHelper.BuildPropertyExpression(typeof(TEntity), filterField) as LambdaExpression, filteValue);
        public static IQueryable<TEntity> FilteBy<TEntity>(this IQueryable<TEntity> entities,
                                                           LambdaExpression filterFieldExpression,
                                                           object filteValue)
        {
            Check.NotNull(filterFieldExpression, nameof(filterFieldExpression));
            Check.NotNull(entities, nameof(entities));
            var propertyType = ExpressionHelper.GetPropertyType(filterFieldExpression);
            var propertyValue = Convert.ChangeType(filteValue, propertyType);
            return entities.Where(Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(filterFieldExpression.Body, Expression.Constant(propertyValue)), filterFieldExpression.Parameters.Single()));
        }
        public static IQueryable<TEntity> FilteBy<TEntity>(this IQueryable<TEntity> entities,
                                                           IEnumerable<string> filterFields,
                                                           object filteValue)
            => Check.NotNull(entities, nameof(entities)).FilteBy(filterFields.Select(a => ExpressionHelper.BuildPropertyExpression(typeof(TEntity), a) as LambdaExpression), filteValue);
        public static IQueryable<TEntity> FilteBy<TEntity>(this IQueryable<TEntity> entities,
                                                           IEnumerable<LambdaExpression> filterFieldExpressions,
                                                           object filteValue)
        {
            Check.NotNull(filterFieldExpressions, nameof(filterFieldExpressions));
            Check.NotNull(entities, nameof(entities));
            foreach (var expression in filterFieldExpressions)
                entities = entities.FilteBy(expression, filteValue);
            return entities;
        }
        #endregion 

        #region LikeAny
        public static IQueryable<TEntity> LikeAny<TEntity>(this IQueryable<TEntity> searchSource,
                                                            string searchKeyField,
                                                            IEnumerable<string> searchPattern,
                                                            char[] seperators = null)
            => Check.NotNull(searchSource, nameof(searchSource)).LikeAny(ExpressionHelper.BuildPropertyExpression(typeof(TEntity), searchKeyField) as LambdaExpression, searchPattern, seperators);

        /// <summary>
        /// Search the source entity list (searchPattern will be splitted by default seperators)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="searchSource"></param>
        /// <param name="searchKeyExpression"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> LikeAny<TEntity>(this IQueryable<TEntity> searchSource,
                                                            Expression<Func<TEntity, string>> searchKeyExpression,
                                                            IEnumerable<string> searchPattern,
                                                            char[] seperators = null)
            => Check.NotNull(searchSource, nameof(searchSource)).LikeAny(searchKeyExpression, searchPattern, seperators);
        

        /// <summary>
        /// Search the source entity list (searchPattern will be splitted by seperators).
        /// But if the seperators parameter is NULL, the searchPattern won't be splitted.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="searchSource"></param>
        /// <param name="searchKeyExpression"></param>
        /// <param name="searchPattern"></param>
        /// <param name="seperators">Except seperators==NULL, others will follow the string.Split() behaviors</param>
        /// <returns></returns>
        public static IQueryable<TEntity> LikeAny<TEntity>(this IQueryable<TEntity> searchSource,
                                                           LambdaExpression searchKeyExpression,
                                                           IEnumerable<string> searchPattern,
                                                         char[] seperators = null
                                                          )
        {
            Check.NotNull(searchKeyExpression, nameof(searchKeyExpression));
            Check.NotNull(searchSource, nameof(searchSource));
            if (!searchPattern.Any())
                return searchSource.Take(0);
            seperators = seperators ?? DefaultSeperators;

            List<string> temp = new List<string>();
            foreach (var pattern in searchPattern)
            {
                if (!string.IsNullOrEmpty(pattern.Trim()))
                {
                    // I have changed this default logic, in order to sometimes we only need search the whole pattern.
                    // But, the default logic for String.Split():
                    // Although we give the parameter with: NULL, or Empty char array to String.Split()
                    // It'll split the source string with " ", which is not expected to us.(We expected search the whole pattern).
                    var tmp = seperators == null ? new string[] { pattern } : pattern.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Any())
                    {
                        foreach (var splitBySpaceCharacter in tmp)
                        {
                            if (!string.IsNullOrEmpty(splitBySpaceCharacter.Trim()))
                            {
                                temp.Add(splitBySpaceCharacter.Trim());
                            }
                        }
                    }
                }
            }
            if (!temp.Any())
            {
                return searchSource.Take(0);
            }
            else
            {
                searchPattern = temp;
            }

            var toLowerExpression = Expression.Call(searchKeyExpression.Body, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            var notNullExpression = Expression.NotEqual(searchKeyExpression.Body, Expression.Constant(null));

            var expression = searchPattern.Select(v =>
                                                   Expression.Equal(Expression.Call(toLowerExpression, typeof(string).GetMethod("Contains"), Expression.Constant(v.ToLowerInvariant())),
                                                                    Expression.Constant(true)
                                                   ));
            var expressionAccumulation = expression.Aggregate((acc, exp) => Expression.OrElse(acc, exp));

            expressionAccumulation = Expression.AndAlso(notNullExpression, expressionAccumulation);

            return searchSource.Where(Expression.Lambda<Func<TEntity, bool>>(expressionAccumulation, searchKeyExpression.Parameters.Single()));
        }
        #endregion

        #region LikeAnyAnd
        public static IQueryable<TEntity> LikeAnyAnd<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<string> searchKeyFields,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators = null)
        {
            Check.NotNull(searchSource, nameof(searchSource));
            foreach (var searchField in searchKeyFields)
                searchSource = searchSource.LikeAny(searchField, searchPattern, seperators);
            return searchSource;
        }
        public static IQueryable<TEntity> LikeAnyAnd<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<LambdaExpression> searchKeyFields,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators = null)
        {
            Check.NotNull(searchSource, nameof(searchSource));
            foreach (var searchFieldExpression in searchKeyFields)
                searchSource = searchSource.LikeAny(searchFieldExpression, searchPattern, seperators);
            return searchSource;
        }
        #endregion

        #region LikeAnyOr
        public static IQueryable<TEntity> LikeAnyOr<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<string> searchKeyFields,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators = null)
            => Check.NotNull(searchSource, nameof(searchSource)).LikeAnyOr(searchKeyFields.Select(a => ExpressionHelper.BuildPropertyExpression(typeof(TEntity), a) as LambdaExpression), searchPattern, seperators);
        
        public static IQueryable<TEntity> LikeAnyOr<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<LambdaExpression> searchKeyExpressions,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators = null
                                                         )
        {
            Check.NotNull(searchKeyExpressions, nameof(searchKeyExpressions));
            Check.NotNull(searchSource, nameof(searchSource));
            if (!searchPattern.Any())
                return searchSource.Take(0);
            seperators = seperators ?? DefaultSeperators;


            List<string> temp = new List<string>();
            foreach (var pattern in searchPattern)
            {
                if (!string.IsNullOrEmpty(pattern.Trim()))
                {
                    // I have changed this default logic, in order to sometimes we only need search the whole pattern.
                    // But, the default logic for String.Split():
                    // Although we give the parameter with: NULL, or Empty char array to String.Split()
                    // It'll split the source string with " ", which is not expected to us.(We expected search the whole pattern).
                    var tmp = seperators == null ? new string[] { pattern } : pattern.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Any())
                    {
                        foreach (var splitBySpaceCharacter in tmp)
                        {
                            if (!string.IsNullOrEmpty(splitBySpaceCharacter.Trim()))
                            {
                                temp.Add(splitBySpaceCharacter.Trim());
                            }
                        }
                    }
                }
            }
            if (!temp.Any())
            {
                return searchSource.Take(0);
            }
            else
            {
                searchPattern = temp;
            }
            searchPattern = searchPattern.Select(a => a.ToLower()).ToList();

            var stringComparer = StringComparer.CurrentCultureIgnoreCase;
            var containsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string)});
            var startWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            //var sqlLikeMethod = typeof(System.Data.Linq.SqlClient.SqlMethods).GetMethod("Like", new Type[]{typeof(string),typeof(string)});

            var accumulationResultExpression = default(Expression<Func<TEntity, bool>>);

            // As each searchKeyExpression has different parameter, we should replace all the parameters with the same one.
            foreach (var searchKeyExpression in searchKeyExpressions)
            { 
                var toLowerExpression = Expression.Call(searchKeyExpression.Body, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                var notNullExpression = Expression.NotEqual(searchKeyExpression.Body, Expression.Constant(null));

                var expression = searchPattern.Select(v =>
                                                       Expression.Equal(
                                                                        Expression.Call(toLowerExpression, containsMethod, Expression.Constant(v)),
                                                                        //Expression.Call(searchKeyExpression.Body, containsMethod, Expression.Constant(v)),  // We need to use ToLower to make it case insensitive.
                                                                        Expression.Constant(true)
                                                       ));
                var expressionAccumulation = expression.Aggregate((acc, exp) => Expression.OrElse(acc, exp));

                expressionAccumulation = Expression.AndAlso(notNullExpression, expressionAccumulation);

                var current = Expression.Lambda<Func<TEntity, bool>>(expressionAccumulation, searchKeyExpression.Parameters.Single());

                // Join the result with OR
                if (accumulationResultExpression == default(Expression<Func<TEntity, bool>>))
                {
                    accumulationResultExpression = current;
                }
                else
                {
                    accumulationResultExpression = accumulationResultExpression.Or(current);
                }
                //Expression<Func<TEntity,bool>> kk= Expression.Lambda<Func<TEntity, bool>>(accumulationResultExpression, parameterExpression);               
            }
            if (searchKeyExpressions.Count() > 0)
            {
                return searchSource.Where(accumulationResultExpression);
            }
            return searchSource;
        }

        public static IQueryable<TEntity> LikeAnyOr<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<Expression<Func<TEntity, string>>> searchKeyExpressions,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators = null)
            => Check.NotNull(searchSource, nameof(searchSource)).LikeAnyOr(searchKeyExpressions.Cast<LambdaExpression>(), searchPattern, seperators);
      

        public static IQueryable<TEntity> LikeAnyOr<TEntity>(this IQueryable<TEntity> searchSource,
                                                          IEnumerable<string> searchPattern,
                                                          char[] seperators,
                                                          params Expression<Func<TEntity, string>>[] searchKeyExpressions
                                                         )
        => Check.NotNull(searchSource, nameof(searchSource)).LikeAnyOr(searchKeyExpressions, searchPattern, seperators);
        #endregion

        #endregion

        #region IEnumerable Extensions

        /// <summary>
        /// Paging the entities
        /// </summary>
        /// <typeparam name="TEntity">Generic Entity Type</typeparam>
        /// <param name="entities"></param>
        /// <param name="skipCount">skip count before fetching page entities</param>
        /// <param name="pageSize">page size</param>
        /// <param name="totalCount">the total count of entities, if totalCount isn't set, or is less than -1, totalCount will be reset with entities.Count()</param>
        /// <returns></returns>
        public static IEnumerable<TEntity> Page<TEntity>(this IQueryable<TEntity> entities, int skipCount, int pageSize, int totalCount = -1)
           where TEntity : class
        {
            if (totalCount <= -1)
                totalCount = entities.Count();

            if (totalCount <= skipCount)
                skipCount = 0;

            var query = entities.Skip(skipCount).Take(pageSize);
            var i = 0;
            for (; i < skipCount; i++)
            {
                yield return null;
            }
            foreach (var entity in query)
            {
                i++;
                yield return entity;
            }
            for (; i < totalCount; i++)
            {
                yield return null;
            }
        }

        public static IEnumerable<TEntity> Page<TEntity>(this IEnumerable<TEntity> entities, int skipCount, int pageSize)
            where TEntity : class
        {
            var count = entities.Count();
            if (count <= skipCount)
                skipCount = 0;
            var query = entities.Skip(skipCount).Take(pageSize);
            var i = 0;
            for (; i < skipCount; i++)
            {
                yield return null;
            }
            foreach (var entity in query)
            {
                i++;
                yield return entity;
            }
            for (; i < count; i++)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Search the source entity list (searchPattern will be splitted by default seperators)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="searchSource"></param>
        /// <param name="searchKeyExpression"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static IEnumerable<TEntity> LikeAny<TEntity>(this IEnumerable<TEntity> searchSource,
                                                            Expression<Func<TEntity, string>> searchKeyExpression,
                                                            IEnumerable<string> searchPattern
                                                           )
        {
            return searchSource.LikeAny(searchKeyExpression, searchPattern, DefaultSeperators);
        }

        /// <summary>
        /// Search the source entity list (searchPattern will be splitted by seperators).
        /// But if the seperators parameter is NULL, the searchPattern won't be splitted.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="searchSource"></param>
        /// <param name="searchKeyExpression"></param>
        /// <param name="searchPattern"></param>
        /// <param name="seperators">Except seperators==NULL, others will follow the string.Split() behaviors</param>
        /// <returns></returns>
        public static IEnumerable<TEntity> LikeAny<TEntity>(this IEnumerable<TEntity> searchSource,
                                                           Expression<Func<TEntity, string>> searchKeyExpression,
                                                           IEnumerable<string> searchPattern,
                                                           char[] seperators
                                                          )
        {
            if (searchKeyExpression == null)
                throw new ArgumentNullException("searchKeyExpression");
            if (searchPattern == null)
                throw new ArgumentNullException("searchPattern");
            if (!searchPattern.Any())
            {
                return searchSource.Take(0);
            }

            List<string> temp = new List<string>();
            foreach (var pattern in searchPattern)
            {
                if (!string.IsNullOrEmpty(pattern.Trim()))
                {
                    // I have changed this default logic, in order to sometimes we only need search the whole pattern.
                    // But, the default logic for String.Split():
                    // Although we give the parameter with: NULL, or Empty char array to String.Split()
                    // It'll split the source string with " ", which is not expected to us.(We expected search the whole pattern).
                    var tmp = seperators == null ? new string[] { pattern } : pattern.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Any())
                    {
                        foreach (var splitBySpaceCharacter in tmp)
                        {
                            if (!string.IsNullOrEmpty(splitBySpaceCharacter.Trim()))
                            {
                                temp.Add(splitBySpaceCharacter.Trim());
                            }
                        }
                    }
                }
            }
            if (!temp.Any())
            {
                return searchSource.Take(0);
            }
            else
            {
                searchPattern = temp;
            }

            var toLowerExpression = Expression.Call(searchKeyExpression.Body, typeof(string).GetMethod("ToLowerInvariant"));
            var notNullExpression = Expression.NotEqual(searchKeyExpression.Body, Expression.Constant(null));

            var expression = searchPattern.Select(v =>
                                                   Expression.Equal(Expression.Call(toLowerExpression, typeof(string).GetMethod("Contains"), Expression.Constant(v.ToLowerInvariant())),
                                                                    Expression.Constant(true)
                                                   ));
            var expressionAccumulation = expression.Aggregate((acc, exp) => Expression.OrElse(acc, exp));

            expressionAccumulation = Expression.AndAlso(notNullExpression, expressionAccumulation);

            return searchSource.Where(Expression.Lambda<Func<TEntity, bool>>(expressionAccumulation, searchKeyExpression.Parameters.Single()).Compile());
        }

        public static IEnumerable<TEntity> Random<TEntity>(this IEnumerable<TEntity> originEntities, int? randomSeed = null)
        {
            if (originEntities == null)
                throw new ArgumentNullException("originEntities is null");

            if (randomSeed == null)
            {
                randomSeed = DateTime.Now.Millisecond;
            }

            Random random = new Random(randomSeed.Value);
            var entities = originEntities.ToList();
            var totalCount = entities.Count;

            for (var i = 0; i < totalCount; i++)
            {
                int index = random.Next(0, totalCount);
                entities.Add(entities[index]);
                entities.RemoveAt(index);
            }

            return entities.AsEnumerable();
        }
        #endregion

        #region Other Code Example
        /// <summary>
        /// this is an example for cleaner way
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="sortField"></param>
        /// <param name="ascending"></param>
        /// <returns></returns>
        static IQueryable<T> OrderByField<T>(this IQueryable<T> obj, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T));
            var prop = Expression.Property(param, sortField);
            var lambdaExp = Expression.Lambda(prop, param);
            var types = new Type[] { obj.ElementType, lambdaExp.Body.Type };
            var lambda = Expression.Call(typeof(Queryable), ascending ? "OrderBy" : "OrderByDescending", types, obj.Expression, lambdaExp);

            return obj.Provider.CreateQuery<T>(lambda);
        }
        #endregion
    }
}
