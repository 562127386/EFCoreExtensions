using EFCoreExtensions.Exceptions;
using EFCoreExtensions.Models;
using EFCoreExtensions.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace EFCoreExtensions.Test
{
    [ExcludeFromCodeCoverage]
    public class LinqExtensionsTest
    {
        private readonly TestContext Context;
        private const int TotalElements = 100;
        public LinqExtensionsTest()
        {
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            Context = options.Initialize(TotalElements);
        }
        [Fact]
        public void GetPaged_PagedParamBase_Source_Thows_ArgumentNullException()
        {
            var source = Context.People;
            source = null;
            Assert.Throws<ArgumentNullException>(() => source.GetPaged(null));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Param_Thows_ArgumentNullException()
        {
            var source = Context.People;
            Assert.Throws<ArgumentNullException>(() => source.GetPaged(null));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Param_Page_Thows_ArgumentOutOfRangeException()
        {
            var param = new PagedParamBase
            {
                Page = -1,
                PageSize = -1
            };
            var source = Context.People;
            Assert.Throws<ArgumentOutOfRangeException>(() => source.GetPaged(param));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Param_Page2_Thows_ArgumentOutOfRangeException()
        {
            var param = new PagedParamBase
            {
                Page = TotalElements,
                PageSize = 10
            };
            var source = Context.People;
            Assert.Throws<ArgumentOutOfRangeException>(() => source.GetPaged(param));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Param_PageSize_Thows_ArgumentOutOfRangeException()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = -1
            };
            var source = Context.People;
            Assert.Throws<ArgumentOutOfRangeException>(() => source.GetPaged(param));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10
            };
            var first = Context.People.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_OrderByAsc_Empty()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Ordering = new KeyValuePair<string, Ordering>(string.Empty, Ordering.Ascending)
            };
            var first = Context.People.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_OrderByAsc_FirstName()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Ordering = new KeyValuePair<string, Ordering>(nameof(PersonEntity.FirstName), Ordering.Ascending)
            };
            var first = Context.People.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_OrderByDesc_FirstName()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Ordering = new KeyValuePair<string, Ordering>(nameof(PersonEntity.FirstName), Ordering.Descending)
            };
            var data = Context.People.OrderByDescending(x => x.FirstName);
            var first = data.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Key_Throw_FilterNullException()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = null,
                        Value = null,
                        Comparison = ComparisonOperators.StartWith,
                    }
                }
            };
            Assert.Throws<FilterNullException>(() => Context.People.GetPaged(param));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Value_Throw_FilterNullException()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = "SomeThing",
                        Value = null,
                        Comparison = ComparisonOperators.StartWith,
                    }
                }
            };
            Assert.Throws<FilterNullException>(() => Context.People.GetPaged(param));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_KeyProperty_Not_Exist()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = "SomeThing",
                        Value = "Value",
                        Comparison = ComparisonOperators.None,
                    }
                }
            };
            var first = Context.People.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Value_Not_Same_Type()
        {
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.IdPerson),
                        Value = "Value",
                        Comparison = ComparisonOperators.None,
                    }
                }
            };
            var first = Context.People.FirstOrDefault();
            var result = Context.People.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(TotalElements, result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Gender_Equals()
        {
            var stringValue = "M";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.Gender),
                        Value = stringValue,
                        Comparison = ComparisonOperators.Equals,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.Gender.Equals('M'));
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Age_NotEquals()
        {
            var stringValue = "20";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.Age),
                        Value = stringValue,
                        Comparison = ComparisonOperators.NotEquals,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.Age != 20);
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Birthday_LessThan()
        {
            var date = new DateTime(1990, 10, 20);
            var stringValue = date.ToString("o");
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.Birthday),
                        Value = stringValue,
                        Comparison = ComparisonOperators.LessThan,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.Birthday < date);
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_Birthday_LessThanOrEqual()
        {
            var date = new DateTime(1990, 10, 20);
            var stringValue = date.ToString("o");
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.Birthday),
                        Value = stringValue,
                        Comparison = ComparisonOperators.LessThanOrEquals
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.Birthday <= date);
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_IdPerson_GreaterThan()
        {
            var stringValue = "80";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.IdPerson),
                        Value = stringValue,
                        Comparison = ComparisonOperators.GreaterThan
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.IdPerson > 80);
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_IdPerson_GreaterThanOrEqual_IdPerson_LessThanOrEqual()
        {
            var stringValue = "1";
            var stringValue2 = "10";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.IdPerson),
                        Value = stringValue,
                        Comparison = ComparisonOperators.GreaterThanOrEquals
                    },
                    new Filter
                    {
                        Key = nameof(PersonEntity.IdPerson),
                        Value = stringValue2,
                        Comparison = ComparisonOperators.LessThanOrEquals,
                        Operator = Operators.AndAlso
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => x.IdPerson >= 1 && x.IdPerson <= 10);
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void WhereEquals_IdPerson()
        {
            var first = Context.People.Where(x => x.IdPerson == 1).FirstOrDefault();
            var result = Context.People.WhereEquals(nameof(PersonEntity.IdPerson), "1").FirstOrDefault();
            Assert.True(first.Equal(result));
        }
        [Fact]
        public void WhereNotEquals_IdPerson()
        {
            var first = Context.People.Where(x => x.IdPerson != 1).FirstOrDefault();
            var result = Context.People.WhereNotEquals(nameof(PersonEntity.IdPerson), "1").FirstOrDefault();
            Assert.True(first.Equal(result));
        }
        [Fact]
        public void WhereLike_FirsName_StartWith()
        {
            var stringValue = "Firstname 1";
            var first = Context.People.Where(x => EF.Functions.Like(x.FirstName, $"{stringValue}%")).FirstOrDefault();
            var result = Context.People.WhereLike(nameof(PersonEntity.FirstName), stringValue, LikeOptions.StartWith).FirstOrDefault();
            Assert.True(first.Equal(result));
        }
        [Fact]
        public void WhereLike_FirsName_Contains()
        {
            var stringValue = "name 10";
            var first = Context.People.Where(x => EF.Functions.Like(x.FirstName, $"%{stringValue}%")).FirstOrDefault();
            var result = Context.People.WhereLike(nameof(PersonEntity.FirstName), stringValue, LikeOptions.Contains).FirstOrDefault();
            Assert.True(first.Equal(result));
        }
        [Fact]
        public void WhereLike_FirsName_EndWith()
        {
            var stringValue = "me 11";
            var first = Context.People.Where(x => EF.Functions.Like(x.FirstName, $"%{stringValue}")).FirstOrDefault();
            var result = Context.People.WhereLike(nameof(PersonEntity.FirstName), stringValue, LikeOptions.EndWith).FirstOrDefault();
            Assert.True(first.Equal(result));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_FirstName_StartWith()
        {
            var stringValue = "Firstname 1";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.FirstName),
                        Value = stringValue,
                        Comparison = ComparisonOperators.StartWith,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => EF.Functions.Like(x.FirstName, $"{stringValue}%"));
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_FirstName_Contains()
        {
            var stringValue = "name 10";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.FirstName),
                        Value = stringValue,
                        Comparison = ComparisonOperators.Contains,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => EF.Functions.Like(x.FirstName, $"%{stringValue}%"));
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
        [Fact]
        public void GetPaged_PagedParamBase_Page_1_PageSize_10_Filter_FirstName_EndWith()
        {
            var stringValue = "me 11";
            var param = new PagedParamBase
            {
                Page = 1,
                PageSize = 10,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        Key = nameof(PersonEntity.FirstName),
                        Value = stringValue,
                        Comparison = ComparisonOperators.EndWith,
                    }
                }
            };
            var source = Context.People;
            var data = source.Where(x => EF.Functions.Like(x.FirstName, $"%{stringValue}"));
            var first = data.FirstOrDefault();
            var result = source.GetPaged(param);
            Assert.Equal(param.PageSize, result.PageSize);
            Assert.Equal(data.Count(), result.RowCount);
            Assert.Equal(param.Page, result.CurrentPage);
            Assert.True(first.Equal(result.Results.FirstOrDefault()));
        }
    }
}
