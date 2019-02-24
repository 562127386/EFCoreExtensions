// <copyright file="LinqExpressions.cs" company="Jorge Jimenez">
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
using EFCoreExtensions.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EFCoreExtensions
{
    /// <summary>
    /// Linq Expression Helper for dynamics expressions
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class LinqExpressions
    {
        /// <summary>
        /// Property value equals to strValue expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression EqualsExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            var result = converter.ConvertFromString(strValue);
            PropertyInfo propertyInfo = typeof(TSource).GetProperty(property);
            MethodInfo equalMethod = typeof(Expression).GetMethod(nameof(Expression.Equal), new[] { typeof(Expression), typeof(Expression) });
            Expression exp = (Expression)equalMethod.Invoke(null, new object[] { Expression.Property(parameter, propertyInfo), Expression.Constant(result) });
            return exp;
        }
        /// <summary>
        /// Property value not equals to strValue experssion
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression NotEqualsExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            var result = converter.ConvertFromString(strValue);
            PropertyInfo propertyInfo = typeof(TSource).GetProperty(property);
            MethodInfo equalMethod = typeof(Expression).GetMethod(nameof(Expression.NotEqual), new[] { typeof(Expression), typeof(Expression) });
            Expression exp = (Expression)equalMethod.Invoke(null, new object[] { Expression.Property(parameter, propertyInfo), Expression.Constant(result) });
            return exp;
        }
        /// <summary>
        /// Property value less than to strValue expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression LessThanExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            object objValue = converter.ConvertFromString(strValue);
            PropertyInfo propInfo = typeof(TSource).GetProperty(property);
            MethodInfo method = typeof(Expression).GetMethod(nameof(Expression.LessThan), new[] { typeof(Expression), typeof(Expression) });
            Expression expression = (Expression)method.Invoke(null, new object[] { Expression.Property(parameter, propInfo), Expression.Constant(objValue) });
            return expression;
        }
        /// <summary>
        /// Property value less than or equal to strValue expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression LessThanOrEqualExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            object objValue = converter.ConvertFromString(strValue);
            PropertyInfo propInfo = typeof(TSource).GetProperty(property);
            MethodInfo method = typeof(Expression).GetMethod(nameof(Expression.LessThanOrEqual), new[] { typeof(Expression), typeof(Expression) });
            Expression expression = (Expression)method.Invoke(null, new object[] { Expression.Property(parameter, propInfo), Expression.Constant(objValue) });
            return expression;
        }
        /// <summary>
        /// Property value greater than to strValue expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression GreaterThanExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            object objValue = converter.ConvertFromString(strValue);
            PropertyInfo propInfo = typeof(TSource).GetProperty(property);
            MethodInfo method = typeof(Expression).GetMethod(nameof(Expression.GreaterThan), new[] { typeof(Expression), typeof(Expression) });
            Expression expression = (Expression)method.Invoke(null, new object[] { Expression.Property(parameter, propInfo), Expression.Constant(objValue) });
            return expression;
        }
        /// <summary>
        /// Property value greater than or equal to strValue expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static Expression GreaterThanOrEqualExpression<TSource>(ParameterExpression parameter, Type type, string property, string strValue)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            object objValue = converter.ConvertFromString(strValue);
            PropertyInfo propInfo = typeof(TSource).GetProperty(property);
            MethodInfo method = typeof(Expression).GetMethod(nameof(Expression.GreaterThanOrEqual), new[] { typeof(Expression), typeof(Expression) });
            Expression expression = (Expression)method.Invoke(null, new object[] { Expression.Property(parameter, propInfo), Expression.Constant(objValue) });
            return expression;
        }
        /// <summary>
        /// Property value like start, end or contains words expression
        /// </summary>
        /// <param name="props"></param>
        /// <param name="words"></param>
        /// <param name="option"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        internal static Expression<Func<TSource, bool>> LikesExpression<TSource>(string[] props, string[] words, LikeOptions option, Operators operators)
        {
            Expression result = null;
            ParameterExpression parameter = Expression.Parameter(typeof(TSource));
            foreach (var prop in props)
            {
                Expression expr = words
                    .Select(word =>
                       LikeExpression<TSource>(parameter, prop, word, option)
                    )
                    .Aggregate<MethodCallExpression, Expression>(null, (current, call) => current != null ? ConcatExpression(current, call, operators) : (Expression)call);
                result = result.ConcatExpression(expr, operators);
            }
            return Expression.Lambda<Func<TSource, bool>>(result, parameter);
        }
        /// <summary>
        /// Property value like start, end or contains strValue expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="property"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        internal static MethodCallExpression LikeExpression<TSource>(ParameterExpression parameter, string property, string strValue, LikeOptions option)
        {
            MethodCallExpression body = Expression.Call(typeof(DbFunctionsExtensions).GetMethod(nameof(DbFunctionsExtensions.Like),
                    new[] {
                        typeof (DbFunctions), typeof (string), typeof (string)
                    }),
                Expression.Constant(EF.Functions),
                Expression.Property(parameter, typeof(TSource).GetProperty(property)),
                Expression.Constant(SetOption(strValue, option)));
            return body;
        }
        /// <summary>
        /// Concat Expression left with right
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operators"></param>
        /// <returns></returns>
        internal static Expression ConcatExpression(this Expression left, Expression right, Operators operators)
        {
            if (left == null)
            {
                return right;
            }
            switch (operators)
            {
                case Operators.AndAlso:
                    {
                        return Expression.AndAlso(left, right);
                    }
                case Operators.And:
                    {
                        return Expression.And(left, right);
                    }
                case Operators.OrElse:
                    {
                        return Expression.OrElse(left, right);
                    }
                case Operators.Or:
                    {
                        return Expression.Or(left, right);
                    }
                case Operators.None:
                    {
                        return left;
                    }
            }
            return left;
        }
        /// <summary>
        /// Set like option Contains, StartWith or EndWith
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        internal static string SetOption(string strValue, LikeOptions option)
        {
            switch (option)
            {
                case LikeOptions.Contains:
                    {
                        strValue = string.Join("", '%', strValue, '%');
                        break;
                    }
                case LikeOptions.StartWith:
                    {
                        strValue = string.Join("", strValue, '%');
                        break;
                    }
                case LikeOptions.EndWith:
                    {
                        strValue = string.Join("", '%', strValue);
                        break;
                    }
            }
            return strValue;
        }
        /// <summary>
        /// Where method call expression extension
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        internal static IQueryable<TSource> WhereExpression<TSource>(this IQueryable<TSource> source, ParameterExpression parameter, Expression comparison)
        {
            // call Where expression to execute the query
            MethodCallExpression whereCallExpression;
            if (comparison != null)
            {
                whereCallExpression = source.WhereCallExpression(parameter, comparison);
            }
            else
            {
                whereCallExpression = source.WhereCallExpression(parameter, Expression.Constant(true));
            }
            return source.Provider.CreateQuery<TSource>(whereCallExpression);
        }
        /// <summary>
        /// Where method call expression extension
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="parameter"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        internal static MethodCallExpression WhereCallExpression<TSource>(this IQueryable<TSource> source,
            ParameterExpression parameter, Expression comparison) =>
            Expression.Call(
                    typeof(Queryable),
                    nameof(Queryable.Where),
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Lambda<Func<TSource, bool>>(comparison, new ParameterExpression[] { parameter })
                );
    }
}
