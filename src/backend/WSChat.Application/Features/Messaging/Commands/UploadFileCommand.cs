namespace WSChat.Application.Features.Messaging.Commands;

using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

public record UploadFileCommand(IFormFile File) : IRequest<string>;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, string>
{
    public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        if (request.File is null)
            return string.Empty;

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);
        var fileDirectory = Path.Combine(Path.GetFullPath("wwwroot"), "uploads");

        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);

        var filePath = Path.Combine(fileDirectory, fileName);

        using (FileStream fileStream = new(filePath, FileMode.OpenOrCreate))
        {
            await request.File.CopyToAsync(fileStream, cancellationToken);
        }

        var fileUrl = Path.Combine("uploads", fileName);

        return fileUrl;
    }
}