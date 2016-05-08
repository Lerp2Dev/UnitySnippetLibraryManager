using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using StringBuilder = System.Text.StringBuilder;
using GUIStyle = UnityEngine.GUIStyle;
using GUIContent = UnityEngine.GUIContent;
using Debug = UnityEngine.Debug;

public class WikiContent
{
    public static string ContentMultiFormatter(string innerHtml, Dictionary<string, string> css = null) //Css debe ser minimificado (detectarlo?)
    {
        bool isCode = innerHtml.Contains("<span");
        string sb = "";
        string[] wordSplitted = innerHtml.Replace(" ", "¸").Replace("<", " <").Replace(">", "> ").Split(new char[1] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        string lastStyle = "";
        foreach (string tag in wordSplitted)
        {
            string word = tag.Replace("†", "\n").Replace("¸", " ");
            string style = "";
            bool isTag = (word.Contains("<") || word.Contains("</")) && word.Contains(">");
            if (word.Contains("</") && word.Contains(">"))
                style = lastStyle;
            else if (word.Contains("<") && word.Contains(">") && !word.Contains("</"))
            {
                string cla = GetClass(word);
                if (!string.IsNullOrEmpty(cla) && css != null && css.Count > 0 && css.ContainsKey(cla))
                    style = css[cla];
            }
            if (isTag)
                word = ContentFormatter(word, style);
            if (!isCode)
                word = word.TrimEnd(new char[] { '\r', '\n' });
            sb += word;
            if(!string.IsNullOrEmpty(style))
                lastStyle = style;
        }
        return sb;
    }
    private static string GetClass(string tag)
    {
        if (tag.Contains(@"class="""))
            return Regex.Match(tag, @"(?<=class="").+?(?="")").ToString();
        else
            return "";
    }
    public static string ContentFormatter(string tag, string css = "")
    {
        HtmlDocument html = new HtmlDocument();
        html.LoadHtml(tag);
        HtmlNode node = html.DocumentNode.FirstChild;
        Dictionary<string, string> cssvalues = new Dictionary<string, string>();
        if(!string.IsNullOrEmpty(css))
            foreach(string p in css.Split(';'))
            {
                string[] v = p.Split(':');
                cssvalues.Add(v[0], v[1]);
            }
        if (tag[1] != '/')
        { //Node es para ir obteniendo las propiedades, aunque ahora que lo pienso no las necesito "↗"
            if (tag.StartsWith("<a"))
                return "<color=#5882FAFF><i>" + (node.Attributes["href"].Value.Contains("http") ? "<size=16>↖</size> " : "");
            else if (tag.StartsWith("<span"))
                return string.Format("<color={0}>{1}{2}",
                    cssvalues.ContainsKey("color") ? (cssvalues["color"].Contains("#") ? cssvalues["color"].ToUpper() + "FF" : cssvalues["color"]) : "#000000FF",
                    cssvalues.ContainsKey("font-style") && cssvalues["font-style"] == "italic" ? "<i>" : "",
                    cssvalues.ContainsKey("font-weight") && cssvalues["font-weight"] == "bold" ? "<b>" : "");
        }
        else
        {
            if (tag == "</a>")
                return "</i></color>";
            else if (tag == "</span>")
                return string.Format("{0}{1}</color>",
                    cssvalues.ContainsKey("font-weight") && cssvalues["font-weight"] == "bold" ? "</b>" : "",
                    cssvalues.ContainsKey("font-style") && cssvalues["font-style"] == "italic" ? "</i>" : "");
        }
        return "";
    }

    //Por el momento no haré nada de css to guistyle

    public static Dictionary<string, WikiLink> GetLinks(string innerHtml)
    {
        Dictionary<string, WikiLink> r = new Dictionary<string, WikiLink>();
        MatchCollection matches = Regex.Matches(innerHtml, @"\<a.+?\>.+?\</a\>");
        foreach (Match match in matches)
            foreach (Capture capture in match.Captures)
            {
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(capture.Value);
                HtmlNode node = html.DocumentNode.FirstChild;
                string u = node.Attributes["href"].Value;
                if (!r.ContainsKey(node.InnerHtml)) //Esto en proximas versiones lo intentaré solventar
                    r.Add(node.InnerHtml, new WikiLink((!u.Contains("http") ? Wiki.mainUrl : "") + u, u.Contains("http")));
                else
                    Debug.LogWarning("Trying to insert link that already exists!\n" + node.InnerHtml);
            }
        if (r.Count > 0)
            return r;
        else
            return null;
    }

    //Funciones checkfor (no necesarias por el momento, creo)

    public static string Unformat(string t)
    {
        MatchCollection matches = Regex.Matches(t, @"\<.+?\>");
        StringBuilder sb = new StringBuilder(t);
        foreach (Match match in matches)
            foreach (Capture capture in match.Captures)
                sb.Replace(capture.Value, "");
        return sb.ToString();
    }

    public static List<string> CuttedLines(string t, float maxwidth, GUIStyle gs)
    {
        if (string.IsNullOrEmpty(t))
            return null;
        string sum = "", ssum = "";
        string[] strs = t.Split(' ');
        float rest = 0;
        List<string> r = new List<string>();
        foreach (string str in strs)
        {
            float w = gs.CalcSize(new GUIContent(sum)).x;
            if (w - rest >= maxwidth)
            {
                r.Add(ssum);
                rest = w;
                ssum = "";
            }
            sum += str + " ";
            ssum += str + " ";
        }
        return r;
    }
}

public class WikiMixedContent : WikiTags //En vez de WikiParagraphTag y WikiListTag
{
    public string FormattedText;
    public Dictionary<string, WikiLink> Links = new Dictionary<string, WikiLink>();
    public WikiMixedContent(string ft)
    {
        FormattedText = ft;
        Text = WikiContent.Unformat(ft);
    }
    public WikiMixedContent(string ft, Dictionary<string, WikiLink> l)
    {
        FormattedText = ft;
        Links = l;
    }
}

public class WikiLink
{
    public string Url;
    public bool External;
    public WikiLink(string u, bool e)
    {
        Url = u;
        External = e;
    }
}//225