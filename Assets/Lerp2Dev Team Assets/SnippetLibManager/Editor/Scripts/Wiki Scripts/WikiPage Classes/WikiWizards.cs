using HtmlAgilityPack;

public class WikiWizards : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph };
    public static WikiWizards GetContents(WikiParams wp)
    {
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children();
        WikiWizards ww = new WikiWizards();
        ww.Contents = WikiTags.GetTags(conts, types, doc);
        return ww;
    }
}