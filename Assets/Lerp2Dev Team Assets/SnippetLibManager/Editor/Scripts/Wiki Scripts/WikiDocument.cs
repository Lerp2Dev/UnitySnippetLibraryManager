using System.Text.RegularExpressions;
using System.Linq;
using HtmlAgilityPack;

public enum ScriptType { JS, CSharp, Boo, C, Text }
public class WikiDocument : WikiPage
{
    private static readonly TagType[] types = new TagType[1] { TagType.All };
    public static WikiDocument GetContents(WikiParams wp)
    {
        Wiki.scrScroll = UnityEngine.Vector2.zero;
        string url = wp.Url;
        var doc = new HtmlWeb()
            .Load(url);
        var conts = doc
            .GetElementbyId("mw-content-text")
            .Children();
        WikiDocument ws = new WikiDocument();
        ws.Contents = WikiTags.GetTags(conts, types, doc);
        return ws;
    }

    public static ScriptType GetScriptType(string c)
    {
        if (c == "csharp source-csharp")
            return ScriptType.CSharp;
        else if (c == "javascript source-javascript")
            return ScriptType.JS;
        else if (c == "boo source-boo")
            return ScriptType.Boo;
        else if (c == "c source-c")
            return ScriptType.C;
        else
            return ScriptType.Text;
    }

    public static string GetScriptCaption(ScriptType t)
    {
        return string.Format("{0} code", t.ToString());
    }

    public static string GetScriptExtension(ScriptType t)
    {
        if (t == ScriptType.Boo)
            return ".boo";
        else if (t == ScriptType.CSharp)
            return ".cs";
        else if (t == ScriptType.JS)
            return ".js";
        else if (t == ScriptType.C)
            return ".shader";
        else if (t == ScriptType.Text)
            return ".txt";
        else
            return "";
    }

}//73