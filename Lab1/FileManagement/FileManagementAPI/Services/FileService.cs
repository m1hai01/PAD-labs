using FileManagementAPI.Database;
using FileManagementAPI.Entities;


namespace FileManagementAPI.Services
{
    // FileService Service
    public class FileService
    {
        private readonly FileDbContext _context;

        public FileService(FileDbContext context)
        {
            _context = context;
        }

        public Entities.File GetFileById(int fileId)
        {
            return _context.Files.Find(fileId);
        }
    }

}
