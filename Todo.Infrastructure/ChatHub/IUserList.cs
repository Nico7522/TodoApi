using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public interface IUserList
{
    void SetOnline(string userId);
    void SetOffline(string userId);

    void SetPresence(string userId);

    List<UserStatus> GetList();
}
