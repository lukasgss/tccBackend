using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjections;

public static class AwsSetup
{
	public static void ConfigureAws(this IServiceCollection services, IConfiguration configuration)
	{
		AwsCredentialData awsCredentials = configuration.GetSection("AWSCredentials").Get<AwsCredentialData>() ??
		                                   throw new Exception("Não foi possível obter as credenciais AWS.");
		AwsData awsData = configuration.GetSection("AWS").Get<AwsData>()
		                  ?? throw new Exception("Não foi possível obter os dados AWS.");
		var awsOptions = new AWSOptions
		{
			Credentials = new BasicAWSCredentials(awsCredentials.AccessKey, awsCredentials.SecretKey),
			Region = Amazon.RegionEndpoint.SAEast1
		};

		var s3Config = new AmazonS3Config()
		{
			ServiceURL = awsData.ServiceUrl
		};

		services.AddDefaultAWSOptions(awsOptions);

		services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(awsOptions.Credentials, s3Config));
	}
}