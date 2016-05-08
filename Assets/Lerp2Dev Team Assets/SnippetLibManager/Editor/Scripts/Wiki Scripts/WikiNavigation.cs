using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

public enum NavigationType { MainNavigation, ExtensionsNavigation, ScriptNavigation }
public class WikiNavigation : WikiPage
{
    public static WikiNavigation GetContents(WikiParams wp)
    {
        NavigationType nt = wp.NavType;
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var c = doc
            .GetElementbyId(GetNavigation(nt))
            .Children();
        HtmlNodeCollection conts = null;
        if (nt == NavigationType.MainNavigation)
            conts = c[1].Children()[0].Children();
        else if (nt == NavigationType.ScriptNavigation || nt == NavigationType.ExtensionsNavigation)
            conts = c[0].Children()[0].Children();
        WikiNavigation wn = new WikiNavigation();
        if (nt == NavigationType.ExtensionsNavigation)
            wn.Contents.Add(new WikiNavigationTag("Plugins", "http://wiki.unity3d.com/index.php/Extension"));
        //else if (nt == NavigationType.ScriptNavigation) //Borrar si eso, existe el botón atrás
        //    wn.Contents.Add(new WikiNavigationTag("Main Script Page", "http://wiki.unity3d.com/index.php/Scripts"));
        foreach (HtmlNode h in conts)
            if (nt == NavigationType.MainNavigation && AllowedSections.Contains(h.FirstChild.InnerText))
                wn.Contents.Add(new WikiNavigationTag(h.FirstChild.InnerText, string.Format("{0}{1}", Wiki.mainUrl, h.FirstChild.Attributes["href"].Value)) { ClassName = GetClassName(h.FirstChild.InnerText) });
            else if (nt == NavigationType.ExtensionsNavigation && !h.InnerText.Contains(" &#160;") && !h.InnerHtml.Contains(@"class=""selflink"""))
                wn.Contents.Add(new WikiNavigationTag(Regex.Replace(h.Children()[0].InnerText, "^[ ]+", ""), string.Format("{0}{1}", Wiki.mainUrl, h.Children()[0].Attributes["href"].Value.Replace(" ", ""))));
            else if (nt == NavigationType.ScriptNavigation && !h.InnerText.Contains(" &#160;"))
                wn.Contents.Add(new WikiNavigationTag(h.Children()[0].InnerText.Replace(" ", "").Replace("/", " "), string.Format("{0}{1}", Wiki.mainUrl, h.Children()[0].Attributes["href"].Value.Replace(" ", ""))));
        return wn;
    }

    private static string GetNavigation(NavigationType n)
    {
        if (n == NavigationType.MainNavigation)
            return "p-Navigation";
        else if (n == NavigationType.ScriptNavigation || n == NavigationType.ExtensionsNavigation)
            return "mw-content-text";
        else
            return "";
    }

    private static string GetClassName(string n)
    {
        if (n != "Scripts")
            return "Wiki" + n.Replace(" ", "");
        else
            return "WikiScriptContainer";
    }
}//52