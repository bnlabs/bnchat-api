using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace ToffApi.Services.CloudFlareR2Service;

public class R2Service : IR2Service
{
    private const string Bucket = "pfp";
    private readonly IAmazonS3 _s3Client;

    public R2Service(string accessKey, string secretKey, string r2Url)
    {
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = r2Url
        });

    }

    public async Task<string> UploadObject(IFormFile file)
    {
        var key = file.FileName.Replace(" ", string.Empty);
        var transferUtility = new TransferUtility(_s3Client);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = file.OpenReadStream(),
            BucketName = Bucket,
            Key = key,
            DisablePayloadSigning = true
        };
        await transferUtility.UploadAsync(uploadRequest);
        
        return $"https://pub-b0e080cfcdcc4f508a242564465e3975.r2.dev/{key}";
    }
    
}