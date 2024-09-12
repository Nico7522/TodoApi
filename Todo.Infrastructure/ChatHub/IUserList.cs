using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public interface IUserList
{
    void SetOnline(string userId, bool isOnline, bool isPresent);
    void SetOffline(string userId, bool isOnline, bool isPresent);

    void SetPresence(string userId);

    List<UserStatus> GetList();
}
