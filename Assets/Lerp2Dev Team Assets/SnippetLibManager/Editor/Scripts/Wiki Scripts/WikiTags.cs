using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Linq;

public enum TagType { All, Heading, TextCode, Code, List, Paragraph }
public class WikiTags
{
    public string Text;
        
    public WikiNavigationTag Nav
    {
        get
        {
            return (WikiNavigationTag)this;
        }
    }

    public WikiCodeTag CCode
    {
        get
        {
            return (WikiCodeTag)this;
        }
    }

    public WikiListTag List
    {
        get
        {
            return (WikiListTag)this;
        }
    }

    public WikiHeaderTag Head
    {
        get
        {
            return (WikiHeaderTag)this;
        }
    }

    public WikiParagraphTag Par
    {
        get
        {
            return (WikiParagraphTag)this;
        }
    }

    public static List<WikiTags> GetTags(HtmlNodeCollection hnc, TagType[] types, HtmlDocument doc = null)
    {
        List<WikiTags> wt = new List<WikiTags>();
        if(doc != null) //This is for get heading if required
            wt.Add(new WikiHeaderTag(Regex.Replace(doc.GetElementbyId("firstHeading").Children()[0].InnerText, "^[ ]+", "").Replace("\n", ""), 1));
        int n;
        foreach (HtmlNode h in hnc)
            if ((types[0] == TagType.All || types.Contains(TagType.Heading)) && CheckForHeading(h, out n))
                wt.Add(new WikiHeaderTag(h.InnerText, n));
            else if ((types[0] == TagType.All || types.Contains(TagType.Paragraph)) && CheckForParagraph(h))
                wt.Add(new WikiParagraphTag(h.InnerHtml.Replace("<br>", "").Replace("<b>", "").Replace("</b>", "")));
            else if ((types[0] == TagType.All || types.Contains(TagType.Code)) && CheckForCode(h))
            {
                ScriptType st = WikiDocument.GetScriptType(h.Children()[0].Attributes["class"].Value);
                var c = h.Children()[0].Children()[0];
                wt.Add(new WikiCodeTag(st, c.InnerHtml, c.InnerText));
            }
            else if ((types[0] == TagType.All || types.Contains(TagType.TextCode)) && CheckForTextCode(h))
                wt.Add(new WikiCodeTag(ScriptType.Text, h.InnerText, h.InnerText));
            else if ((types[0] == TagType.All || types.Contains(TagType.List)) && CheckForList(h)) //Esto irá evolucionando para obtener elementos de forma recursiva
                wt.Add(new WikiListTag(h.Children(), h.Name == "ol"));
        return wt;
    }

    private static bool CheckForHeading(HtmlNode h, out int n)
    {
        n = 0;
        return h.Name.Length == 2 && h.Name[0] == 'h' && int.TryParse(h.Name[1].ToString(), out n);
    }

    private static bool CheckForParagraph(HtmlNode h)
    {
        return h.Name == "p" && (!string.IsNullOrEmpty(h.InnerText) || !h.InnerText.IsNullOrWhiteSpace() || h.InnerHtml != "<br>");
    }

    private static bool CheckForCode(HtmlNode h)
    {
        return h.Name == "div" && h.Attributes["class"] != null && h.Attributes["class"].Value.Contains("mw-geshi");
    }

    private static bool CheckForTextCode(HtmlNode h)
    {
        return h.Name == "pre";
    }

    private static bool CheckForList(HtmlNode h)
    {
        return h.Name == "ul" || h.Name == "ol";
    }
}//243