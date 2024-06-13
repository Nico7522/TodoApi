using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Queries.GetTasksByUser;

internal class GetTasksByUserQueryHandler : IRequestHandler<GetTasksByUserQuery, IEnumerable<TodoEntity>>
{

    private readonly IUserRepository _userRepository;
    public GetTasksByUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IEnumerable<TodoEntity>> Handle(GetTasksByUserQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetTasksByUser(request.UserId);
    }
}
