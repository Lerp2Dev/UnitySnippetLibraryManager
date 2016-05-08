using HtmlAgilityPack;

public class WikiScriptContainer : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph };
    public static WikiScriptContainer GetContents(WikiParams wp) //cl = catchLinks
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
        WikiScriptContainer wsc = new WikiScriptContainer();
        wsc.Contents = WikiTags.GetTags(conts, types, !url.Contains("Scripts/") ? doc : null);
        return wsc;
    }
}//47