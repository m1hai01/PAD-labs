using UserManagementAPI.Database;
using UserManagementAPI.Entities;

namespace UserManagementAPI.Services
{
    // UserService Service
    public class UserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }
    }

}
