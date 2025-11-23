using System.Diagnostics.CodeAnalysis;

namespace Domain.Common;

[ExcludeFromCodeCoverage]
public sealed class FileAttachment
{
    [Obsolete]
    private FileAttachment()
    {
    }

    public FileAttachment(string fileUrl, string fileName)
    {
        FileUrl = fileUrl;
        FileName = fileName;
    }

    public string FileUrl { get; init; } = null!;
    public string FileName { get; init; } = null!;
}