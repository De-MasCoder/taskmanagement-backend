
using TaskManagement.Application.Interfaces;
using TaskContracts.Events;
using MassTransit;

namespace TaskFileUploadService.Consumers
{
    public class FileUploadConsumer : IConsumer<FileUploadRequested>
    {
        private readonly IFileStorageService _fileStorage;

        public FileUploadConsumer(IFileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task Consume(ConsumeContext<FileUploadRequested> context)
        {
            var message = context.Message;

            using var stream = new MemoryStream(message.FileBytes);

            var fileUrl = await _fileStorage.UploadFileAsync(
                stream, message.FileName, message.ContentType);

            // Todo : Notify client that file is uploaded successfully
            Console.WriteLine($"File uploaded for Task {message.TaskId}: {fileUrl}");
        }
    }
}
