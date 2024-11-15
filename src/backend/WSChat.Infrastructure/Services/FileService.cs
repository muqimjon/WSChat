namespace WSChat.Infrastructure.Services;

using WSChat.Application.Interfaces;

public class FileService : IFileService
{
    private readonly string storagePath;

    public FileService()
    {
        storagePath = Path.Combine(Path.GetFullPath("wwwroot"), "uploads");

        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }
    }

    public async Task<string> UploadFileAsync(byte[] fileData, string fileName)
    {
        var filePath = Path.Combine(storagePath, fileName);
        await File.WriteAllBytesAsync(filePath, fileData);
        return filePath;
    }
}
