# EFCoreExtensions

## Installing EFCoreExtensions
[EFCoreExtensions](https://www.nuget.org/packages/EFCoreExtensions/) can be downloaded from NuGet.

# Usage Examples

## GetPaged page 1 with page size of 10 Example
```csharp
...
...
...

PagedResult<Entity> pagedResult = new PagedResult<Entity>();

using(var dbContext = new DbContext())
{
  var pagedParameters = new PagedParamBase
  {
    Page = 1,
    PageSize = 10
  }
  pagedResult = dbContext.Entity.GetPaged(pagedParameters);
}

List<Entity> listResult = pagedResult.Results;

...
...
...
```

## GetPaged page 1 with page size of 10 OrderByAsc Entity Id Example
```csharp
...
...
...

PagedResult<Entity> pagedResult = new PagedResult<Entity>();

using(var dbContext = new DbContext())
{
  var pagedParameters = new PagedParamBase
  {
    Page = 1,
    PageSize = 10,
    Ordering = new KeyValuePair<string, Ordering>(nameof(Entity.Id), Ordering.Ascending)
  }
  pagedResult = dbContext.Entity.GetPaged(pagedParameters);
}

List<Entity> listResult = pagedResult.Results;

...
...
...
```

## GetPaged page 1 with page size of 10 OrderByDesc Entity Id Example
```csharp
...
...
...

PagedResult<Entity> pagedResult = new PagedResult<Entity>();

using(var dbContext = new DbContext())
{
  var pagedParameters = new PagedParamBase
  {
    Page = 1,
    PageSize = 10,
    Ordering = new KeyValuePair<string, Ordering>(nameof(Entity.Id), Ordering.Descending)
  }
  pagedResult = dbContext.Entity.GetPaged(pagedParameters);
}

List<Entity> listResult = pagedResult.Results;

...
...
...
```

## GetPaged page 1 with page size of 10 Filter Entity Id Example
```csharp
...
...
...

PagedResult<Entity> pagedResult = new PagedResult<Entity>();

using(var dbContext = new DbContext())
{
  var stringValue = "1";
  var pagedParameters = new PagedParamBase
  {
    Page = 1,
    PageSize = 10,
    Filters = new List<Filter>
    {
      new Filter
      {
        Key = nameof(Entity.Id),
        Value = stringValue,
        Comparison = ComparisonOperators.Equals,
      }
    }
  }
  pagedResult = dbContext.Entity.GetPaged(pagedParameters);
}

List<Entity> listResult = pagedResult.Results;

...
...
...
```

## WhereLike StartWith Example
```csharp
...
...
...

List<Entity> listResult = new List<Entity>();
var stringValue = "John";
using(var dbContext = new DbContext())
{
  listResult = dbContext.Entity.WhereLike(nameof(Entity.FullName), stringValue, LikeOptions.StartWith)
}

...
...
...
```
