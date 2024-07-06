
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Todo.Application.Tests;

public static class MockDbSetExtensions
{
    public static Mock<DbSet<T>> AsDbSetMock<T>(this IEnumerable<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();

        var dbSetMock = new Mock<DbSet<T>>();
        dbSetMock.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        dbSetMock.Setup(d => d.FindAsync(It.IsAny<object[]>()))
            .Returns<object[]>(ids => new ValueTask<T>(queryable.FirstOrDefault(e =>
                e.GetType().GetProperty("Id").GetValue(e).Equals(ids[0]))));

        return dbSetMock;
    }
}
