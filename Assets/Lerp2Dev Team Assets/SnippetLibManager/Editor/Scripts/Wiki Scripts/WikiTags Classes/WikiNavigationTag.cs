using HtmlAgilityPack;

public class WikiNavigationTag : WikiTags
{
    public string Url;
    public string ClassName;
    public WikiNavigationTag(string t, string u)
    {
        Text = HtmlEntity.DeEntitize(t);
        Url = u;
    }
}