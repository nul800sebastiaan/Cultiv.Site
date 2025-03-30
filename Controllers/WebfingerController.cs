using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Cultiv.Controllers;

[ApiController]
[Route(".well-known/[controller]")]
[Produces("application/json")]
public class WebfingerController : ControllerBase
{
    [HttpGet]
    public ActionResult Get([FromQuery] string resource)
    {
        if (string.IsNullOrEmpty(resource) || resource != "acct:hello@cultiv.nl")
        {
            return new NotFoundResult(); 
        }

        var value = new WebfingerModel
        {
            Subject = "acct:sebastiaan@cultiv.social",
            Aliases = new List<string>
            {
                "https://cultiv.social/@sebastiaan",
                "https://cultiv.social/user/sebastiaan"
            },
            Links = new List<Link>
            {
                new Link
                {
                    Rel = "http://webfinger.net/rel/profile-page",
                    Type = "text/html",
                    Href = "https://cultiv.social/@sebastiaan"
                },
                new Link
                {
                    Rel = "self",
                    Type = "application/activity+json",
                    Href = "https://cultiv.social/users/sebastiaan"
                },
                new Link
                {
                    Rel = "http://ostatus.org/schema/1.0/subscribe",
                    Template = "https://mastodon.social/authorize_interaction?uri={uri}",
                }
            }
        };

        return new JsonResult(value);
    }
}

public class Link
{
    [JsonProperty("rel")]
    [JsonPropertyName("rel")]
    public string Rel { get; set; }

    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonProperty("template")]
    [JsonPropertyName("template")]
    public string Template { get; set; }
}

public class WebfingerModel
{
    [JsonProperty("subject")]
    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonProperty("aliases")]
    [JsonPropertyName("aliases")]
    public List<string> Aliases { get; set; }

    [JsonProperty("links")]
    [JsonPropertyName("links")]
    public List<Link> Links { get; set; }
}