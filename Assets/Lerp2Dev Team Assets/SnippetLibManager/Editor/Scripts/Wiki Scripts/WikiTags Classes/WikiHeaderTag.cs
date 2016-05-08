using HtmlAgilityPack;

public class WikiHeaderTag : WikiTags
{
    public int HeaderSize;
    public WikiHeaderTag(string t, int hs)
    {
        Text = HtmlEntity.DeEntitize(t);
        HeaderSize = hs;
    }
}