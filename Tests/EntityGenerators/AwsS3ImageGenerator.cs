using Application.Common.Interfaces.ExternalServices;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators;

public static class AwsS3ImageGenerator
{
    public static AwsS3FileResponse GenerateSuccessS3ImageResponse()
    {
        return new AwsS3FileResponse()
        {
            Success = Constants.S3ImagesData.Success,
            PublicUrl = Constants.S3ImagesData.PublicUrl
        };
    }

    public static AwsS3FileResponse GenerateFailS3ImageResponse()
    {
        return new AwsS3FileResponse()
        {
            Success = false,
            PublicUrl = null
        };
    }
}