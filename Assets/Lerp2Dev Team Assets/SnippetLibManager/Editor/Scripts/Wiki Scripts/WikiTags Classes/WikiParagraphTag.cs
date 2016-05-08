using HtmlAgilityPack;

public class WikiParagraphTag : WikiTags
{
    public WikiMixedContent MixedContent;
    public WikiParagraphTag(string ft) //ft = Formatted text
    {
        ft = HtmlEntity.DeEntitize(ft);
        MixedContent = new WikiMixedContent(WikiContent.ContentMultiFormatter(ft), WikiContent.GetLinks(ft));
    }
}