using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Cultiv.Site.Extensions;
using Cultiv.Site.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Cultiv.Controllers;

public class BlogOverviewController : RenderController
{
    private readonly IVariationContextAccessor _variationContextAccessor;
    private readonly ServiceContext _serviceContext;
        
    public BlogOverviewController(ILogger<BlogOverviewController> logger, 
        ICompositeViewEngine compositeViewEngine, 
        IUmbracoContextAccessor umbracoContextAccessor,
        IVariationContextAccessor variationContextAccessor, 
        ServiceContext context)
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        _variationContextAccessor = variationContextAccessor;
        _serviceContext = context;
    }
        
    [HttpGet]
    public IActionResult Index([FromQuery(Name = "page")] int page = 1)
    {
        var publishedValueFallback = new PublishedValueFallback(_serviceContext, _variationContextAccessor);
        var model = new BlogOverviewModel(CurrentPage, publishedValueFallback) { Page = page };
        var allBlogPosts = model.Children.ToList();
            
        var skip = 10 * model.Page - 10;
            
        model.TotalPages = Convert.ToInt32(Math.Ceiling(allBlogPosts.Count / 10.0));
        model.PreviousPage = model.Page - 1;
        model.NextPage = model.Page + 1;
        model.IsFirstPage = model.Page <= 1;
        model.IsLastPage = model.Page >= model.TotalPages;
            
        var selection = allBlogPosts.OrderByDescending(x => x.CreateDate)
            .Skip(skip)
            .Take(10)
            .Select(publishedContent => publishedContent as BlogPost)
            .ToList();

        model.PagedBlogPosts = selection;

        return CurrentTemplate(model);
    }
    
    //[ResponseCache(Duration = 1200)]
    [HttpGet]
    public IActionResult BlogRss()
    {
        var blogCopyright = $"{DateTime.Now.Year} Sebastiaan Janssen";
        var blogTitle = "Cultiv Blog";
        var blogDescription = "";
        var blogUrl = "https://cultiv.nl/";
        var blogSlug = "blog";
        var feedId = "RSSUrl";
        
        var feed = new SyndicationFeed(blogTitle, blogDescription, new Uri(blogUrl), feedId, DateTime.Now)
        {
            Copyright = new TextSyndicationContent(blogCopyright)
        };
        
        var items = new List<SyndicationItem>();
        
        var publishedValueFallback = new PublishedValueFallback(_serviceContext, _variationContextAccessor);
        var model = new BlogOverviewModel(CurrentPage, publishedValueFallback);
        var posts = model.Children.OrderByDescending(x => x.CreateDate)
            .Take(20)
            .Select(publishedContent => publishedContent as BlogPost)
            .ToList();
        
        foreach (var blogPost in posts)
        {
            var item = new SyndicationItem(blogPost.Name, blogPost.BodyText.GetSummary(50).ToString(), new Uri($"{blogUrl}/{blogSlug}/{blogPost.UrlSegment}"))
            {
                PublishDate = blogPost.CreateDate,
                Id = blogPost.UrlSegment
            };
            items.Add(item);
        }
        
        feed.Items = items;
        
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true
        };

        using var stream = new MemoryStream();
        using (var xmlWriter = XmlWriter.Create(stream, settings))
        {
            var rssFormatter = new Rss20FeedFormatter(feed, false);
            rssFormatter.WriteTo(xmlWriter);
            xmlWriter.Flush();
        }
        
        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }
}