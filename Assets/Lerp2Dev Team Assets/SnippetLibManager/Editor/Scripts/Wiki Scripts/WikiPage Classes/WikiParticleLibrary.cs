using HtmlAgilityPack;

public class WikiParticleLibrary : WikiPage
{
    private static readonly TagType[] types = new TagType[3] { TagType.Heading, TagType.List, TagType.Paragraph };
    public static WikiParticleLibrary GetContents(WikiParams wp)
    {
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children();
        WikiParticleLibrary wpl = new WikiParticleLibrary();
        wpl.Contents = WikiTags.GetTags(conts, types, doc);
        return wpl;
    }
}