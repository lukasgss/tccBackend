namespace Application.Common.Interfaces.General.Files;

public interface IImageProcessingService
{
    Task<MemoryStream> CompressImageAsync(Stream image);
}