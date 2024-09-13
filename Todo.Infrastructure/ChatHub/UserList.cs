
using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public class UserList : IUserList
{
    private List<UserStatus> _userList = new List<UserStatus>();
    public List<UserStatus> UsersList => _userList;

    public void SetOnline(string userId)
    {
        var user = _userList.FirstOrDefault(u => u.Id == userId);
        if (user is null) _userList.Add(new UserStatus() { Id = userId, IsOnline = true, IsPresent = false });

        if (user is not null)
        {
            user.IsOnline = true;
        }
    }

    public void SetOffline(string userId)
    {
        var user = _userList.FirstOrDefault(u => u.Id == userId);
        if(user is not null)
        {
            user.IsOnline = false;
        };
    }

    public void SetPresence(string userId)
    {
        var user = _userList.FirstOrDefault(u => u.Id == userId);
        if (user is null) _userList.Add(new UserStatus() { Id = userId, IsOnline = false, IsPresent = true });
        if (user is not null)
        {
            user.IsPresent = true;
        };
    }

    public List<UserStatus> GetList()
    {
       
        return _userList; 
    }
}
