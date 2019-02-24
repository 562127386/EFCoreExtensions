// <copyright file="PagedParamBase.cs" company="Jorge Jimenez">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EFCoreExtensions.Models
{
    /// <summary>
    /// Paged param base model
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PagedParamBase
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public IList<Filter> Filters { get; set; }
        public KeyValuePair<string, Ordering>? Ordering { get; set; }
        public PagedParamBase()
        {
            Filters = new List<Filter>();
        }
    }
    /// <summary>
    /// Filter model
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Filter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public ComparisonOperators Comparison { get; set; }
        public Operators Operator { get; set; } = Operators.None;
    }
}
