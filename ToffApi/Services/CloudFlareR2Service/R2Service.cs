using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace ToffApi.Services.CloudFlareR2Service;

public class R2Service : IR2Service
{
    private const string Bucket = "pfp";
    private readonly IAmazonS3 _s3Client;
    private string BucketUrl { get; set; }

    public R2Service(string accessKey, string secretKey, string r2Url,string bucketUrl)
    {
        var credentials = new BasicAWSCredentials(accessKey, secretKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = r2Url
        });
        BucketUrl = bucketUrl;
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
        
        return $"{BucketUrl}{key}";
    }
}