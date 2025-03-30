using HtmlAgilityPack;
using Umbraco.Cms.Core.Strings;

namespace Cultiv.Site.Extensions;

public static class StringExtensions
{
    public static IHtmlEncodedString GetSummary(this IHtmlEncodedString input, int previewWords)
    {
        var result = string.Empty;
        ;
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(input.ToString());
        var flatText = htmlDocument.DocumentNode.InnerText.Replace("\r", " ").Replace("\n", " ");
            
        var words = flatText.Split(new char[] { ' ' });
        if (words.Length > previewWords)
        {
            for (var i = 0; i < previewWords; i++)
                result = result + words[i] + " ";
        }
        else
        {
            result = flatText;
        }

        var htmlString = new HtmlEncodedString(result).RemoveInlineImageStyles();

        return htmlString;
    }
        
    public static IHtmlEncodedString RemoveInlineImageStyles(this IHtmlEncodedString input)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(input.ToHtmlString());

        var documentNode = htmlDocument.DocumentNode;
        var nodes = documentNode?.SelectNodes("//img[@style]");

        if (nodes == null)
            return input;

        foreach (var htmlNode in (htmlDocument.DocumentNode.SelectNodes("//img[@style]")))
            htmlNode.Attributes.Remove("style");

        return new HtmlEncodedString(htmlDocument.DocumentNode.OuterHtml);
    }

    public static string ConvertNewlinesToHtmlBreaks(string html)
    {
        return html.Replace(Environment.NewLine, "<br />");
    }
}