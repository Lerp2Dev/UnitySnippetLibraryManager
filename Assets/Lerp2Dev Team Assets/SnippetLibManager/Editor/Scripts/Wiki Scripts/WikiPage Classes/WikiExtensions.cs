using HtmlAgilityPack;

public class WikiExtensions : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph };
    public static WikiExtensions GetContents(WikiParams wp)
    {
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children()[0]
            .Children()[1]
            .Children()[0]
            .Children()[0]
            .Children();
        WikiExtensions we = new WikiExtensions();
        we.Contents = WikiTags.GetTags(conts, types, doc);
        return we;
    }
}
