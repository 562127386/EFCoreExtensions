// <copyright file="LikeOptions.cs" company="Jorge Jimenez">
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
namespace EFCoreExtensions.Models
{
    /// <summary>
    /// Like options None, Contains, StartWith, EndWith
    /// </summary>
    public enum LikeOptions
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Contains
        /// </summary>
        Contains = 1,
        /// <summary>
        /// Start with
        /// </summary>
        StartWith = 2,
        /// <summary>
        /// End with
        /// </summary>
        EndWith = 3
    }
    /// <summary>
    /// Like Operators None, Or, Or Else, And, And Else
    /// </summary>
    public enum Operators
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Or
        /// </summary>
        Or = 1,
        /// <summary>
        /// Or else
        /// </summary>
        OrElse = 2,
        /// <summary>
        /// And
        /// </summary>
        And = 3,
        /// <summary>
        /// And else
        /// </summary>
        AndAlso = 4
    }
    /// <summary>
    /// Ordering options
    /// </summary>
    public enum Ordering
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Asc
        /// </summary>
        Ascending = 1,
        /// <summary>
        /// Desc
        /// </summary>
        Descending =2
    }
    /// <summary>
    /// Comparison Operators
    /// </summary>
    public enum ComparisonOperators
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Equals
        /// </summary>
        Equals = 1,
        /// <summary>
        /// Not Equals
        /// </summary>
        NotEquals = 2,
        /// <summary>
        /// Less than
        /// </summary>
        LessThan = 3,
        /// <summary>
        /// Less than or equals
        /// </summary>
        LessThanOrEquals = 4,
        /// <summary>
        /// Greater than
        /// </summary>
        GreaterThan = 5,
        /// <summary>
        /// Greater than or equals
        /// </summary>
        GreaterThanOrEquals = 6,
        /// <summary>
        /// Contains
        /// </summary>
        Contains = 7,
        /// <summary>
        /// Start with
        /// </summary>
        StartWith = 8,
        /// <summary>
        /// End with
        /// </summary>
        EndWith = 9
    }
}
