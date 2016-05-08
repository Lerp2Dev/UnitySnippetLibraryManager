using System.Collections.Generic;
using HtmlAgilityPack;

public class WikiShaders : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph }; //TagType.Text, TagType.Link, TagType.Bold
    public static WikiShaders GetContents(WikiParams wp)
    {
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children();
        WikiShaders ws = new WikiShaders();
        ws.Contents = WikiTags.GetTags(conts, types, doc);
        return ws;
    }
}//28