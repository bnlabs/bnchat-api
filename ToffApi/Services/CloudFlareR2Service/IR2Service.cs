namespace ToffApi.Services.CloudFlareR2Service;

public interface IR2Service
{
    Task<string> UploadObject(IFormFile file);

}