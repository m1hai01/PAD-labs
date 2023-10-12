using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FileManagementAPI.Entities;


namespace FileManagementAPI.Database
{
    // FileService
    public class FileDbContext : DbContext
    {
        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
        }

        // DbSet properties representing your database tables
        public DbSet<Entities.File> Files { get; set; }
    }

}
