
using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public class UserList : IUserList
{
    private List<UserStatus> _userList = new List<UserStatus>();
    public List<UserStatus> UsersList => _userList;

    public void SetOnline(string userId, bool isOnline, bool isPresent)
    {
        var user = _userList.FirstOrDefault(u => u.Id == userId);
        if (user is null) _userList.Add(new UserStatus() { Id = userId, IsOnline = isOnline, IsPresent = isPresent });

        if (user is not null)
        {
            user.IsPresent = isPresent;
            user.IsOnline = isOnline;
        }
    }

    public void SetOffline(string userId, bool isOnline, bool isPresent)
    {
        var user = _userList.FirstOrDefault(u => u.Id == userId);
        if(user is not null)
        {
            user.IsPresent = isPresent;
            user.IsOnline = isOnline;
        };
    }

    public List<UserStatus> GetList()
    {
       
        return _userList; 
    }
}
