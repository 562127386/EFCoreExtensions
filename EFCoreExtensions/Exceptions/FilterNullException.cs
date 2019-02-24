// <copyright file="FilterNullException.cs" company="Jorge Jimenez">
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
using System.Diagnostics.CodeAnalysis;
using EFCoreExtensions.Models;
namespace EFCoreExtensions.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a null reference (Nothing in Visual Basic)
    /// is passed to a <see cref="Filter"/> that does not accept it as a valid argument.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterNullException : ArgumentNullException
    {
        public FilterNullException()
        {
        }
        public FilterNullException(string paramName) : base(paramName)
        {
        }
        public FilterNullException(string paramName, Exception innerException) :
            base(paramName, innerException)
        {
        }
        public FilterNullException(string paramName, string message) :
            base(paramName, message)
        {
        }
    }
}
