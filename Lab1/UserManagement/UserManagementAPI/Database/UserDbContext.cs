using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserManagementAPI.Entities;


namespace UserManagementAPI.Database
{
    // UserService
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        // DbSet properties representing your database tables
        public DbSet<User> Users { get; set; }
    }
}
