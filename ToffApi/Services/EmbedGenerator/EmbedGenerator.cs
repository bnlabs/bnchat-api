using HtmlAgilityPack;
using ToffApi.Models;
using static System.String;

namespace ToffApi.Services.EmbedGenerator;

public class EmbedGenerator
{
    public static List<Embed> GenerateEmbed(string content)
    {
        var resultEmbeds = new List<Embed>();
        var words = content.Split(' ');
        foreach (var word in words)
        {
            // Check if the word resembles a URL
            if (!Uri.TryCreate(word, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) continue;
            // Create a new HtmlWeb instance
            var web = new HtmlWeb();

            // Load the webpage
            var doc = web.Load(uri);

            // Extract the necessary metadata
            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText;
            var description = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']")?.GetAttributeValue("content", "");
            var imageUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.GetAttributeValue("content", "");

            // Create a new UrlPreview object
            var embed = new Embed
            {
                Url = uri.ToString(),
                Title = title,
                Description = description,
                ImageUrl = imageUrl
            };
            if (IsNullOrEmpty(title) && IsNullOrEmpty(description) && IsNullOrEmpty(imageUrl)) { continue; }

            // Add the UrlPreview to the list
            resultEmbeds.Add(embed);
        }
        return resultEmbeds;
    }
    
}