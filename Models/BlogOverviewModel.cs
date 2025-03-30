using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Cultiv.Site.Models;

public class BlogOverviewModel : PublishedContentWrapped 
{
    public BlogOverviewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
    { }

    public int Page { get; set; }

    public int TotalPages { get; set; }

    public int PreviousPage { get; set; }

    public int NextPage { get; set; }

    public bool IsFirstPage { get; set; }

    public bool IsLastPage { get; set; }

    public IEnumerable<BlogPost> PagedBlogPosts { get; set; }
}