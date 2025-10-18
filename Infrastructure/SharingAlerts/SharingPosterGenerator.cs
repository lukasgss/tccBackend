using Application.Common.Exceptions;
using Application.Common.Interfaces.SharingAlerts;
using Ardalis.GuardClauses;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using SixLabors.ImageSharp;
using Image = SixLabors.ImageSharp.Image;

namespace Infrastructure.SharingAlerts;

public class SharingPosterGenerator : ISharingPosterGenerator
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SharingPosterGenerator(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = Guard.Against.Null(httpClientFactory);
    }

    public async Task<MemoryStream> GenerateAdoptionPoster(AdoptionAlertPosterData adoptionAlertPosterData)
    {
        using var stream = new MemoryStream();
        var pdf = new PdfDocument(new PdfWriter(stream));
        Document doc = new(pdf);

        var image = await GetImageFromUrl(adoptionAlertPosterData.Image);
        AdoptionAlertPrintData printData = new(
            image,
            adoptionAlertPosterData.PetName,
            adoptionAlertPosterData.Age,
            adoptionAlertPosterData.Size.Name,
            adoptionAlertPosterData.Sex,
            adoptionAlertPosterData.IsCastrated,
            adoptionAlertPosterData.ContactName,
            adoptionAlertPosterData.ContactPhoneNumber);
        string html = DownloadTemplates.GetShareAdoptionPdfTemplate(printData);
        var elements = HtmlConverter.ConvertToElements(html);

        foreach (var element in elements)
        {
            doc.Add((IBlockElement)element);
        }

        doc.Close();

        return new MemoryStream(stream.ToArray());
    }

    private async Task<string> GetImageFromUrl(string url)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient(ImageStorageApiConfig.ClientKey);
            await using var stream = await httpClient.GetStreamAsync(url);
            using var image = await Image.LoadAsync(stream);
            using var outputStream = new MemoryStream();

            await image.SaveAsJpegAsync(outputStream);
            return Convert.ToBase64String(outputStream.ToArray());
        }
        catch
        {
            throw new InternalServerErrorException("Não foi possível obter a imagem do animal.");
        }
    }
}