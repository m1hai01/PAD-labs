namespace UserManagementAPI.Models
{
    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
    }

}