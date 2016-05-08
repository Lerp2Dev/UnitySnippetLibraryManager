using UnityEngine;
using UnityEditor;
using HtmlAgilityPack;
using System.Linq;
using System;
using System.Collections.Generic;

public static class Helpers
{

    #region "HTML Extensions"

    public static HtmlNodeCollection GetElementsByClassName(this HtmlNode n, string c)
    {
        HtmlNodeCollection h = new HtmlNodeCollection(n),
            hr = new HtmlNodeCollection(n);
        foreach (HtmlNode e in n.Descendants())
            h.Append(e);
        HtmlNode[] hs = h.Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value.Contains(c)).ToArray();
        foreach (HtmlNode r in hs)
            hr.Append(r);
        return hr;
    }

    public static HtmlNodeCollection GetElementsByTagName(this HtmlNode n, string c)
    {
        HtmlNodeCollection h = new HtmlNodeCollection(n),
            hr = new HtmlNodeCollection(n);
        foreach (HtmlNode e in n.Descendants())
            h.Append(e);
        HtmlNode[] hs = h.Where(x => x.Name == c).ToArray();
        foreach (HtmlNode r in hs)
            hr.Append(r);
        return hr;
    }

    public static HtmlNodeCollection GetElementsByName(this HtmlNode n, string c)
    {
        HtmlNodeCollection h = new HtmlNodeCollection(n),
            hr = new HtmlNodeCollection(n);
        foreach (HtmlNode e in n.Descendants())
            h.Append(e);
        HtmlNode[] hs = h.Where(x => x.Attributes.Contains("name") && x.Attributes["name"].Value.Contains(c)).ToArray();
        foreach (HtmlNode r in hs)
            hr.Append(r);
        return hr;
    }

    public static HtmlNode GetElementById(this HtmlNode n, string c)
    {
        HtmlNodeCollection h = new HtmlNodeCollection(n);
        foreach (HtmlNode e in n.Descendants())
            h.Append(e);
        HtmlNode hs = h.FirstOrDefault(x => x.Id == c);
        return hs;
    }

    public static HtmlNodeCollection Children(this HtmlNode h)
    {
        HtmlNodeCollection r = new HtmlNodeCollection(h);
        foreach (HtmlNode n in h.ChildNodes)
            if (n.GetType() == typeof(HtmlNode))
                r.Append(n);
        return r;
    }

    #endregion

    #region "Linq Extensions"

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        foreach (T item in list)
            action(item);
    }

    #endregion

    #region "Math Extensions"

    public static int Evaluate(this string expression)
    {
        string[] m = expression.Split(new char[] { '.' });
        int r = 0;
        foreach (string s in m)
            r += int.Parse(s);
        return r;
    }

    public static bool IsNumeric(this string s)
    {
        float output;
        return float.TryParse(s, out output);
    }

    #endregion

    #region "String Extensions"

    public static bool IsNullOrWhiteSpace(this string value)
    {
        return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
    }

    public static IEnumerable<string> Split(this string str, int chunkSize)
    {
        if (str.Length > chunkSize)
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        else
            return new string[1] { str };
    }

    #endregion

    #region "Texture Extensions"

    public static Texture2D UnitTex(this Color32 col)
    {
        return ((Color)col).UnitTex();
    }

    public static Texture2D UnitTex(this Color col)
    {
        Texture2D result = new Texture2D(1, 1);
        result.SetPixel(0, 0, col);
        result.Apply();
        return result;
    }

    #endregion

    #region "Wiki Extensions"

    public static bool CheckType<T>(this WikiTags w) where T : WikiTags
    {
        return w.GetType() == typeof(T);
    }

    public static bool CheckType<T>(this WikiPage w) where T : WikiPage
    {
        return w.GetType() == typeof(T);
    }

    #endregion
}

static class CustomGUI
{
    public static readonly GUIStyle splitter;
    static CustomGUI()
    {
        splitter = new GUIStyle();
        splitter.normal.background = EditorGUIUtility.whiteTexture;
        splitter.stretchWidth = true;
        splitter.margin = new RectOffset(0, 0, 2, 2);
    }
    private static readonly Color splitterColor = EditorGUIUtility.isProSkin ? new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);
    // GUILayout Style
    public static void Splitter(Color rgb, float thickness = 1)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter, GUILayout.Height(thickness));

        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = rgb;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

    public static void Splitter(Color rgb, float thickness, GUIStyle splitterStyle)
    {
        Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle, GUILayout.Height(thickness));

        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = rgb;
            splitterStyle.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

    public static void Splitter(float thickness = 1)
    {
        Splitter(Color.white, thickness, splitter);
    }

    // GUI Style
    public static void Splitter(Rect position)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color restoreColor = GUI.color;
            GUI.color = splitterColor;
            splitter.Draw(position, false, false, false, false);
            GUI.color = restoreColor;
        }
    }

}

public class HtmlConverter
{
    private static Dictionary<string, string> booColors = new Dictionary<string, string>(),
                                              csColors = new Dictionary<string, string>(),
                                              jsColors = new Dictionary<string, string>(),
                                              cColors = new Dictionary<string, string>();

    //En proximas actualizaciones haré que se obtenga directamente del html de una página
    private static string booCSS = ".kw1{color:green;font-weight:bold;}.kw2{color:navy;}.kw3{color:blue;font-weight:bold;}.kw4{color:#8B4513;}.kw5{color:teal;font-weight:bold;}.kw6{color:blue;font-weight:bold;}.kw7{color:blue;font-weight:bold;}.kw8{color:blue;font-weight:bold;}.kw9{color:maroon;}.kw10{color:maroon;}.kw11{color:purple;}.kw12{color:#4B0082;}.kw13{color:purple;font-weight:bold;}.kw14{color:#008B8B;font-weight:bold;}.kw15{color:brown;}.kw16{color:black;font-weight:bold;}.kw17{color:gray;}.co1{color:#999999;font-style:italic;}.co2{color:#999999;font-style:italic;}.coMULTI{color:#008000;font-style:italic;}.es0{color:#0000FF;font-weight:bold;}.es_h{color:#0000FF;font-weight:bold;}.br0{color:#006400;}.sy0{color:#006400;}.st0{color:#008000;}.st_h{color:#008000;}.nu0{color:#00008B;}.me0{color:000000;}.me1{color:000000;}",
                          csCSS = ".kw1{color:#0600FF;font-weight:bold;}.kw2{color:#FF8000;font-weight:bold;}.kw3{color:#008000;}.kw4{color:#6666cc;font-weight:bold;}.kw5{color:#000000;}.co1{color:#008080;font-style:italic;}.co2{color:#008080;}.co3{color:#008080;}.coMULTI{color:#008080;font-style:italic;}.es0{color:#008080;font-weight:bold;}.es_h{color:#008080;font-weight:bold;}.br0{color:#008000;}.sy0{color:#008000;}.st0{color:#666666;}.st_h{color:#666666;}.nu0{color:#FF0000;}.me1{color:#0000FF;}.me2{color:#0000FF;}",
                          jsCSS = ".kw1{color:#000066;font-weight:bold;}.kw2{color:#003366;font-weight:bold;}.kw3{color:#000066;}.co1{color:#006600;font-style:italic;}.co2{color:#009966;font-style:italic;}.coMULTI{color:#006600;font-style:italic;}.es0{color:#000099;font-weight:bold;}.br0{color:#009900;}.sy0{color:#339933;}.st0{color:#3366CC;}.nu0{color:#CC0000;}.me1{color:#660066;}",
                          cCSS = ".kw1{color:#b1b100;}.kw2{color:#000000;font-weight:bold;}.kw3{color:#000066;}.kw4{color:#993333;}.co1{color:#666666;font-style:italic;}.co2{color:#339933;}.coMULTI{color:#808080;font-style:italic;}.es0{color:#000099;font-weight:bold;}.es1{color:#000099;font-weight:bold;}.es2{color:#660099;font-weight:bold;}.es3{color:#660099;font-weight:bold;}.es4{color:#660099;font-weight:bold;}.es5{color:#006699;font-weight:bold;}.br0{color:#009900;}.sy0{color:#339933;}.st0{color:#ff0000;}.nu0{color:#0000dd;}.nu6{color:#208080;}.nu8{color:#208080;}.nu12{color:#208080;}.nu16{color:#800080;}.nu17{color:#800080;}.nu18{color:#800080;}.nu19{color:#800080;}.me1{color:#202020;}.me2{color:#202020;}";

    public static void Init()
    {
        IEnumerable<string> bs = booCSS.Split('.').Skip(1),
                            cs = csCSS.Split('.').Skip(1),
                            js = jsCSS.Split('.').Skip(1),
                            ccs = cCSS.Split('.').Skip(1);
        foreach (string b in bs)
            booColors.Add(b.Substring(0, b.IndexOf('{')), b.Substring(b.IndexOf('{') + 1).Replace(";}", ""));
        foreach (string c in cs)
            csColors.Add(c.Substring(0, c.IndexOf('{')), c.Substring(c.IndexOf('{') + 1).Replace(";}", ""));
        foreach (string j in js)
            jsColors.Add(j.Substring(0, j.IndexOf('{')), j.Substring(j.IndexOf('{') + 1).Replace(";}", ""));
        foreach (string cc in ccs)
            cColors.Add(cc.Substring(0, cc.IndexOf('{')), cc.Substring(cc.IndexOf('{') + 1).Replace(";}", ""));
    }

    //Preserve bold and italics
    public static WikiCodeTag GetRichText(WikiCodeTag cont)
    {
        Dictionary<string, string> reqCss = new Dictionary<string, string>();
        if (cont.CodeType == ScriptType.Boo)
            reqCss = booColors;
        else if (cont.CodeType == ScriptType.CSharp)
            reqCss = csColors;
        else if (cont.CodeType == ScriptType.JS)
            reqCss = jsColors;
        else if (cont.CodeType == ScriptType.C)
            reqCss = cColors;
        return new WikiCodeTag(cont.CodeType, WikiContent.ContentMultiFormatter(cont.Code, reqCss), cont.RawCode);
    }
}//258