using HtmlAgilityPack;

public class WikiProgramming : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph };
    public static WikiProgramming GetContents(WikiParams wp)
    {
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children();
        WikiProgramming wp1 = new WikiProgramming();
        wp1.Contents = WikiTags.GetTags(conts, types, doc);
        return wp1;
    }
}
