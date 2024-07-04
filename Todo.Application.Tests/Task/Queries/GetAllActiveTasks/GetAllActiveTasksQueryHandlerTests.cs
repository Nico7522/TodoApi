using AutoMapper;
using Moq;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;
namespace Todo.Application.Task.Queries.GetAllActiveTasks.Tests;

public class GetAllActiveTasksQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly GetAllActiveTasksQueryHandler _handler;
    private readonly Mock<IMapper> _mapperMock;
    public GetAllActiveTasksQueryHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllActiveTasksQueryHandler(_todoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact()]
    public async TaskAsync Handle_ForRetrieveAllActiveTask_ShouldReturnAllActiveTask()
    {
        // arrange

        var task1 = new TodoEntity()
        {
            Title = "title1",
            Description = "desc1",
            IsComplete = false,
        };
        var task2 = new TodoEntity()
        {
            Title = "title2",
            Description = "desc2",
            IsComplete = false,

        };

        ICollection<TodoEntity> todoEntities = new List<TodoEntity>() { task1, task2};
        _todoRepositoryMock.Setup(r => r.GetAllActive()).ReturnsAsync(todoEntities);

        var query = new GetAllActiveTasksQuery();

        // act

        await _handler.Handle(query, CancellationToken.None);


        // assert

        _todoRepositoryMock.Verify(r => r.GetAllActive(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TaskDto>>(todoEntities), Times.Once);



    }
}