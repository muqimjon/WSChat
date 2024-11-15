namespace WSChat.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(byte[] fileData, string fileName);
}