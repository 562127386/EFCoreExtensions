// <copyright file="LinqExtensionsMethods.cs" company="Jorge Jimenez">
//    This file is part of EFCoreExtensions library.
//
//    EFCoreExtensions library is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    EFCoreExtensions library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with EFCoreExtensions library.  If not, see https://www.gnu.org/licenses/
// </copyright>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EFCoreExtensions.Exceptions;
using EFCoreExtensions.Models;

namespace EFCoreExtensions
{
    /// <summary>
    /// Entity Framework Core Linq Extensions
    /// </summary>
    public static class LinqExtensionsMethods
    {
        public static PagedResult<TSource> GetPaged<TSource>(this IQueryable<TSource> source,
            PagedParamBase param) where TSource : class
        {
            // Args validations
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (param.Page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(param.Page));
            }

            if (param.PageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(param.PageSize));
            }

            return param.Filters.Any() ?
                source.WhereFiltered(param.Filters).OrderData(param.Ordering).GetPaged(param.Page, param.PageSize) :
                source.OrderData(param.Ordering).GetPaged(param.Page, param.PageSize);

        }        /// <summary>
                 ///  Where property value like %strValue% extension
                 /// </summary>
                 /// <typeparam name="TSource"></typeparam>
                 /// <param name="source"></param>
                 /// <param name="property"></param>
                 /// <param name="strValue"></param>
                 /// <returns></returns>
        public static IQueryable<TSource> WhereLike<TSource>(this IQueryable<TSource> source,
            string property, string strValue, LikeOptions option) =>
            source.WhereFiltered(new List<Filter> {
                new Filter
                {
                    Key = property,
                    Value = strValue,
                    Comparison = option.ParseEnum()
                }
            });
        /// <summary>
        /// Where property value equals to strValue extension
        /// </summary>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereEquals<TSource>(this IQueryable<TSource> source,
            string property, string strValue) =>
            source.WhereFiltered(new List<Filter> {
                new Filter
                {
                    Key = property,
                    Value = strValue,
                    Comparison = ComparisonOperators.Equals
                }
            });
        /// <summary>
        /// Where property value not equals to strValue extension
        /// </summary>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereNotEquals<TSource>(this IQueryable<TSource> source,
            string property, string strValue) =>
            source.WhereFiltered(new List<Filter> {
                new Filter
                {
                    Key = property,
                    Value = strValue,
                    Comparison = ComparisonOperators.NotEquals
                }
            });
        /// <summary>
        /// Order By property ASC
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderByAsc<TSource>(this IQueryable<TSource> source, string property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "x");
            MemberExpression propToOrder = Expression.Property(parameter, property);
            UnaryExpression exprConvert = Expression.Convert(propToOrder, typeof(IComparable));
            // Call expression order by to execute the query
            MethodCallExpression orderByAsc = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.OrderBy),
                new Type[] { source.ElementType, typeof(IComparable) },
                source.Expression,
                Expression.Lambda<Func<TSource, IComparable>>(exprConvert, new ParameterExpression[] { parameter }));
            return source.Provider.CreateQuery<TSource>(orderByAsc);
        }
        /// <summary>
        /// Order by property DESC
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IQueryable<TSource> OrderByDesc<TSource>(this IQueryable<TSource> source, string property)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TSource), "x");
            MemberExpression propToOrder = Expression.Property(parameter, property);
            UnaryExpression exprConvert = Expression.Convert(propToOrder, typeof(IComparable));
            // Call expression order by to execute the query
            MethodCallExpression orderByDesc = Expression.Call(
                typeof(Queryable),
                nameof(Queryable.OrderByDescending),
                new Type[] { source.ElementType, typeof(IComparable) },
                source.Expression,
                Expression.Lambda<Func<TSource, IComparable>>(exprConvert, new ParameterExpression[] { parameter }));
            return source.Provider.CreateQuery<TSource>(orderByDesc);
        }
        /// <summary>
        /// Get source filtered
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereFiltered<TSource>(this IQueryable<TSource> source, IList<Filter> filters)
        {
            FilterValidation(filters);
            Expression expression = null;
            // Defines the parameter
            var parameter = Expression.Parameter(typeof(TSource), "x");
            foreach (var filter in filters)
            {
                var currentExpression = FilterExpression<TSource>(parameter, filter);
                if (currentExpression != null)
                {
                    expression = expression.ConcatExpression(currentExpression, filter.Operator);
                }
            }
            return source.WhereExpression(parameter, expression);
        }
        private static Expression FilterExpression<TSource>(ParameterExpression parameter, Filter filter)
        {
            Expression result = null;
            // Skip if Filter Key isn't property
            if (!PropertyExist<TSource>(filter.Key))
            {
                return result;
            }
            var propInfo = typeof(TSource).GetProperty(filter.Key);
            // Skip if filter value didn't parse
            if (!TryParse(filter.Value, propInfo.PropertyType))
            {
                return result;
            }
            // Set Operation Expression
            switch (filter.Comparison)
            {
                case ComparisonOperators.Equals:
                    {
                        result = LinqExpressions.EqualsExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.NotEquals:
                    {
                        result = LinqExpressions.NotEqualsExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.LessThan:
                    {
                        result = LinqExpressions.LessThanExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.LessThanOrEquals:
                    {
                        result = LinqExpressions.LessThanOrEqualExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.GreaterThan:
                    {
                        result = LinqExpressions.GreaterThanExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.GreaterThanOrEquals:
                    {
                        result = LinqExpressions.GreaterThanOrEqualExpression<TSource>(parameter,
                            propInfo.PropertyType, filter.Key, filter.Value);
                        break;
                    }
                case ComparisonOperators.Contains:
                    {
                        result = LinqExpressions.LikeExpression<TSource>(parameter,
                            filter.Key, filter.Value, LikeOptions.Contains);
                        break;
                    }
                case ComparisonOperators.StartWith:
                    {
                        result = LinqExpressions.LikeExpression<TSource>(parameter,
                            filter.Key, filter.Value, LikeOptions.StartWith);
                        break;
                    }
                case ComparisonOperators.EndWith:
                    {
                        result = LinqExpressions.LikeExpression<TSource>(parameter,
                            filter.Key, filter.Value, LikeOptions.EndWith);
                        break;
                    }
            }
            return result;
        }
        /// <summary>
        /// Validates any filter
        /// </summary>
        /// <param name="filters"></param>
        private static void FilterValidation(IList<Filter> filters)
        {
            foreach (var filter in filters)
            {
                if (string.IsNullOrEmpty(filter.Key))
                {
                    throw new FilterNullException(nameof(filter.Key));
                }
                if (string.IsNullOrEmpty(filter.Value))
                {
                    throw new FilterNullException(nameof(filter.Value));
                }
            }
        }
        /// <summary>
        /// Get source data ordered
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        private static IQueryable<TSource> OrderData<TSource>(this IQueryable<TSource> source,
            KeyValuePair<string, Ordering>? ordering)
        {

            if (!ordering.HasValue || !PropertyExist<TSource>(ordering.Value.Key))
            {
                return source;
            }

            return ordering.Value.Value == Ordering.Ascending ?
                source.OrderByAsc(ordering.Value.Key) :
                source.OrderByDesc(ordering.Value.Key);
        }
        /// <summary>
        /// Get paged data extension from <see cref="IQueryable{TSource}"/>
        /// </summary>
        /// <param name="source">A sequence to get paged data.</param>
        /// <param name="page">A page number to get data.</param>
        /// <param name="pageSize">A page size (rows per page).</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="page" /> and <paramref name="pageSize" />.</exception>
        private static PagedResult<TSource> GetPaged<TSource>(this IQueryable<TSource> source,
            int page, int pageSize) where TSource : class
        {

            var result = new PagedResult<TSource>();
            result.CurrentPage = page;
            result.PageSize = pageSize;

            // Execute query to get total row count
            result.RowCount = source.Count();

            // Set total page count
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);
            if (result.PageCount < page)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            var skip = (page - 1) * pageSize;

            // Execute query to get paginate data
            result.Results = source.Skip(skip).Take(pageSize).ToList();
            return result;
        }
        private static ComparisonOperators ParseEnum(this LikeOptions value)
        {
            return (ComparisonOperators)Enum.Parse(typeof(ComparisonOperators), value.ToString("G"), true);
        }
        /// <summary>
        /// Try Parse any Type
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool TryParse(this string input, Type type)
        {
            bool isConversionSuccessful = false;
            var converter = TypeDescriptor.GetConverter(type);
            if (converter != null)
            {
                try
                {
                    var result = converter.ConvertFromString(input);
                    isConversionSuccessful = true;
                }
                catch { }
            }
            return isConversionSuccessful;
        }
        /// <summary>
        /// PropertyExist
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool PropertyExist<TSource>(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                return false;
            }
            return typeof(TSource).GetProperties().Any(x => x.Name.Equals(property));
        }
    }
}
