using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Type = System.Type;

public enum SearchType { OnAll, OnTitle, OnDescription }

public class Wiki
{

    /*
    
        In the next update:
    
        - Better links
        - Images
        - Tables
        - Code colors from file text (with array using the enum for make everything more dynamic)
        - Show results in script search

    */

    public const bool isDebug = false;

    public const string mainUrl = "http://wiki.unity3d.com";

    private static int curScrNavIndex,
                       curExtsNavIndex;

    public static Vector2 extsScroll = Vector2.zero,
                          scrScroll = Vector2.zero, 
                          scriptScroll = Vector2.zero,
                          shdScroll = Vector2.zero,
                          wzrScroll = Vector2.zero,
                          pgmScroll = Vector2.zero,
                          plibScroll = Vector2.zero;

    public static WikiPage curPage,
                           extsPages,
                           wikiPages,
                           scriptPages;

    private static string strSearch = "", lastSSearch;
    public static bool isSearching;
    private static SearchType sType;

    public static bool readyScr;

    private static GUIStyle linkStyle;

    private static float MinParagraphWidth = -1, MinParagraphHeight = -1;
    private static bool notCalcValues;

    public static List<WikiParams> Initializator = new List<WikiParams>();

    private static Dictionary<string, List<string>> CutsLib = new Dictionary<string, List<string>>();
    private static List<string> nullLib = (new string[1] { "" }).ToList();

    public static void Init()
    {
        Initializator.Add(new WikiParams(NavigationType.MainNavigation, string.Format("{0}/index.php", mainUrl)) { Type = typeof(WikiNavigation) });
        Initializator.Add(new WikiParams(NavigationType.ExtensionsNavigation, string.Format("{0}/index.php/Extensions", mainUrl)) { Type = typeof(WikiNavigation) });
        Initializator.Add(new WikiParams(NavigationType.ScriptNavigation, string.Format("{0}/index.php/Scripts", mainUrl)) { Type = typeof(WikiNavigation) });
        Initializator.Add(new WikiParams(string.Format("{0}/index.php/Scripts", mainUrl)) { Type = typeof(WikiScriptContainer) });
        curPage = WikiPage.Init(typeof(WikiScriptContainer), Initializator.FirstOrDefault(x => x.Type == typeof(WikiScriptContainer)));
        wikiPages = WikiPage.Init(typeof(WikiNavigation), Initializator.FirstOrDefault(x => x.NavType == NavigationType.MainNavigation));
        extsPages = WikiPage.Init(typeof(WikiNavigation), Initializator.FirstOrDefault(x => x.NavType == NavigationType.ExtensionsNavigation));
        scriptPages = WikiPage.Init(typeof(WikiNavigation), Initializator.FirstOrDefault(x => x.NavType == NavigationType.ScriptNavigation));
    }

    public static void Draw()
    {
        GUIStyle normalLabel = new GUIStyle("label") { margin = new RectOffset(0, 0, 4, 0) };
        if (!notCalcValues)
        {
            linkStyle = new GUIStyle("label") { richText = true, wordWrap = true };
            MinParagraphWidth = linkStyle.CalcSize(new GUIContent("")).x;
            MinParagraphHeight = linkStyle.CalcHeight(new GUIContent("A"), 20);
            notCalcValues = true;
        }
        GUILayout.BeginArea(new Rect(5, 0, 125, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (wikiPages != null && wikiPages.Contents != null)
        {
            foreach (WikiTags wt in wikiPages.Contents)
                if (GUILayout.Button(wt.Text, GUILayout.Width(125))) 
                {
                    curPage = WikiPage.Init(Type.GetType(wt.Nav.ClassName), new WikiParams(wt.Nav.Url));
                    if(curPage.CheckType<WikiScriptContainer>() && readyScr)
                        readyScr = false;
                }
        }
        else
            GUILayout.Label("Loading...");
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(135, 25, Screen.width-130, Screen.height-30));
        if(curPage != null)
            if(curPage.CheckType<WikiExtensions>()) //Proxima actualizacion generalizar todo: diferenciar los contaier de los que no son container
            {
                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width - 155));
                GUILayout.Label("Menu:", normalLabel);
                curExtsNavIndex = EditorGUILayout.Popup(curExtsNavIndex, extsPages.Contents.Select(x => x.Text).ToArray(), new GUIStyle("popup") { margin = new RectOffset(0, 0, 3, 0) }, GUILayout.Width(125), GUILayout.Height(20));
                if (GUILayout.Button("Load content", GUILayout.Width(125)))
                    curPage = WikiPage.Init(typeof(WikiExtensions), new WikiParams(extsPages.Contents.ElementAt(curExtsNavIndex).Nav.Url));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Box("", GUILayout.Width(Screen.width - 159), GUILayout.Height(Screen.height - 80));
                GUILayout.BeginArea(new Rect(5, 26, Screen.width - 145, Screen.height - 80));
                extsScroll = GUILayout.BeginScrollView(extsScroll, false, curPage.Contents != null && curPage.Contents.Count > 0);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if (curPage.CheckType<WikiParticleLibrary>())
            {
                GUILayout.Box("", GUILayout.Width(Screen.width - 155), GUILayout.Height(Screen.height - 55));
                GUILayout.BeginArea(new Rect(0, 0, Screen.width - 140, Screen.height - 55));
                plibScroll = GUILayout.BeginScrollView(plibScroll, false, true);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if (curPage.CheckType<WikiProgramming>())
            {
                GUILayout.Box("", GUILayout.Width(Screen.width - 155), GUILayout.Height(Screen.height - 55));
                GUILayout.BeginArea(new Rect(0, 0, Screen.width - 140, Screen.height - 55));
                pgmScroll = GUILayout.BeginScrollView(pgmScroll, false, true);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if (curPage.CheckType<WikiScriptContainer>())
            {
                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width - 155));
                GUILayout.Label("Menu:", normalLabel);
                curExtsNavIndex = EditorGUILayout.Popup(curExtsNavIndex, scriptPages.Contents.Select(x => x.Text).ToArray(), new GUIStyle("popup") { margin = new RectOffset(0, 0, 3, 0) }, GUILayout.Width(125), GUILayout.Height(20));
                if (GUILayout.Button("Load content", GUILayout.Width(125)))
                {
                    curPage = WikiPage.Init(typeof(WikiScriptContainer), new WikiParams(scriptPages.Contents.ElementAt(curExtsNavIndex).Nav.Url));
                    readyScr = true;
                }
                GUILayout.FlexibleSpace();
                if (readyScr)
                {
                    strSearch = GUILayout.TextField(strSearch, new GUIStyle("textfield") { alignment = TextAnchor.MiddleLeft }, GUILayout.Width(150), GUILayout.Height(20));
                    if (strSearch != lastSSearch)
                        isSearching = strSearch.Length > 0;
                    lastSSearch = strSearch;
                    GUILayout.Label("Search on:", normalLabel);
                    sType = (SearchType)EditorGUILayout.EnumPopup(sType, new GUIStyle("popup"), GUILayout.Width(110));
                }
                GUILayout.EndHorizontal();
                GUILayout.Box("", GUILayout.Width(Screen.width-159), GUILayout.Height(Screen.height-80));
                GUILayout.BeginArea(new Rect(5, 26, Screen.width-145, Screen.height-80));
                scrScroll = GUILayout.BeginScrollView(scrScroll, false, curPage.Contents != null && curPage.Contents.Count > 0);
                bool breakedSpliter = false;
                if (readyScr)
                {
                    foreach (WikiTags wt in curPage.Contents)
                    {
                        if (wt.CheckType<WikiHeaderTag>())
                        {
                            if (curPage.Contents.FirstOrDefault() != wt)
                            {
                                if (!breakedSpliter)
                                    GUILayout.Label("No results found.");
                                else
                                    breakedSpliter = false;
                                CustomGUI.Splitter(new Color32(201, 201, 201, 255), 2);
                            }
                            GUILayout.Label(wt.Text, new GUIStyle("label") { fontSize = GetFontSize(wt.Head.HeaderSize), fontStyle = FontStyle.Bold, margin = new RectOffset(5, 0, 0, 0) });
                            if (wt.Head.HeaderSize <= 2)
                                CustomGUI.Splitter(new Color32(201, 201, 201, 255), 2);
                        }
                        else if (wt.CheckType<WikiListTag>())
                            foreach (WikiMixedContent wmc in wt.List.ListContents)
                                if (!isSearching || isSearching && FoundedSearch(wmc.Text, strSearch, sType))
                                {
                                    DisplayMixed(wmc);
                                    breakedSpliter = true;
                                }
                    }
                    if (!breakedSpliter)
                        GUILayout.Label("No results found.");
                }
                else
                    foreach (WikiTags wt in curPage.Contents)
                        DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if (curPage.CheckType<WikiDocument>())
            {
                if (GUILayout.Button("Atrás", GUILayout.Width(75)))
                    curPage = WikiPage.Init(typeof(WikiScriptContainer), new WikiParams(scriptPages.Contents.ElementAt(curExtsNavIndex).Nav.Url));
                GUILayout.Box("", GUILayout.Width(Screen.width - 155), GUILayout.Height(Screen.height - 75));
                GUILayout.BeginArea(new Rect(0, 22, Screen.width - 140, Screen.height - 75));
                scrScroll = GUILayout.BeginScrollView(scrScroll, false, true);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if(curPage.CheckType<WikiShaders>())
            {
                GUILayout.Box("", GUILayout.Width(Screen.width - 155), GUILayout.Height(Screen.height - 55));
                GUILayout.BeginArea(new Rect(0, 0, Screen.width - 140, Screen.height - 55));
                shdScroll = GUILayout.BeginScrollView(shdScroll, false, true);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else if(curPage.CheckType<WikiWizards>())
            {
                GUILayout.Box("", GUILayout.Width(Screen.width - 155), GUILayout.Height(Screen.height - 55));
                GUILayout.BeginArea(new Rect(0, 0, Screen.width - 140, Screen.height - 55));
                wzrScroll = GUILayout.BeginScrollView(wzrScroll, false, true);
                foreach (WikiTags wt in curPage.Contents)
                    DefDisplay(wt);
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width - 130, Screen.height - 30));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Work in progress...");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndArea();
            }
        GUILayout.EndArea();
    }

    public static int GetFontSize(int hs) //Formula
    {
        if (hs == 1)
            return 20;
        else if (hs == 2)
            return 16;
        else if (hs == 3)
            return 14;
        else
            return 12;
    }

    public static bool FoundedSearch(string t, string s, SearchType ty)
    {
        if (t.Length > 0 && s.Length > 0)
            if (ty == SearchType.OnAll)
                return t.Contains(s);
            else if (ty == SearchType.OnTitle)
                return t.IndexOf(" -") > -1 && t.Substring(0, t.IndexOf(" -")).Contains(s);
            else if (ty == SearchType.OnDescription)
                return t.IndexOf("- ") > -1 && t.Substring(t.IndexOf("- ")).Contains(s);
            else
                return false;
        else
            return false;
    }

    private static void DefDisplay(WikiTags wt)
    {
        if (wt.CheckType<WikiHeaderTag>())
        {
            GUILayout.Label(wt.Text, new GUIStyle("label") { fontSize = GetFontSize(wt.Head.HeaderSize), fontStyle = FontStyle.Bold, margin = new RectOffset(5, 0, 0, 0) });
            if (wt.Head.HeaderSize <= 2)
                CustomGUI.Splitter(new Color32(201, 201, 201, 255), 2);
        }
        else if (wt.CheckType<WikiParagraphTag>())
            DisplayMixed(wt.Par.MixedContent);
        else if(wt.CheckType<WikiListTag>())
            foreach (WikiMixedContent wmc in wt.List.ListContents)
                DisplayMixed(wmc);
        else if (wt.CheckType<WikiCodeTag>())
        {
            GUILayout.BeginHorizontal(new GUIStyle("box"));
            GUILayout.Label(WikiDocument.GetScriptCaption(wt.CCode.CodeType));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("See code"))
                CodeViewer.Init(HtmlConverter.GetRichText(wt.CCode));
            GUILayout.EndHorizontal();
        }
    }

    private static void DisplayMixed(WikiMixedContent wmc)
    {
        GUILayout.Label(wmc.FormattedText, new GUIStyle("label") { wordWrap = true, richText = true, margin = new RectOffset(5, 0, 5, 5) }, GUILayout.Width(Screen.width - 175));
        if (wmc.Links != null && wmc.Links.Count > 0)
        {
            if(!string.IsNullOrEmpty(wmc.Text) && !CutsLib.ContainsKey(wmc.Text))
                CutsLib.Add(wmc.Text, WikiContent.CuttedLines(wmc.Text, Screen.width, linkStyle));
            foreach (var kv in wmc.Links)
                DisplayLink(kv.Key, kv.Value, !string.IsNullOrEmpty(wmc.Text) ? CutsLib[wmc.Text] : nullLib, wmc);
        }
    }

    private static void DisplayLink(string ft, WikiLink link, List<string> ps, WikiMixedContent wmc)
    {
        if (ps == null || ps == nullLib)
            return;
        Vector2 d = linkStyle.CalcSize(new GUIContent(ft));
        float xPos = 0, yPos = 0;
        string p = ps.FirstOrDefault(x => x.Contains(ft)),
               cs = "";
        if (string.IsNullOrEmpty(p))
        {
            if (wmc.Text.Contains(" ") && ft.Split(' ')[0] != wmc.Text.Split(' ')[0])
                cs = wmc.Text.Substring(0, wmc.Text.IndexOf(ft));
        }
        else
        {
            if (p.Contains(" ") && ft.Split(' ')[0] != p.Split(' ')[0])
                cs = p.Substring(0, p.IndexOf(ft));
        }
        xPos = linkStyle.CalcSize(new GUIContent(cs)).x - MinParagraphWidth;
        yPos = linkStyle.CalcHeight(new GUIContent(wmc.Text.Substring(0, wmc.Text.IndexOf(ft))), Screen.width) - MinParagraphHeight;
        Rect r = GUILayoutUtility.GetLastRect();
        Rect bt = new Rect(r.xMin + xPos - (link.External ? 8 : 0), r.yMin + yPos + (link.External ? 4 : 0), d.x + (link.External ? 12 : 0), d.y);
        Event e = Event.current;
        if (bt.Contains(e.mousePosition)) // <-- Set pointer (little buggy)
            EditorGUIUtility.AddCursorRect(new Rect(e.mousePosition.x, e.mousePosition.y, 32, 32), MouseCursor.Link);
        if (GUI.Button(bt, "", isDebug ? new GUIStyle("box") : new GUIStyle()))
            if (link.External)
                Application.OpenURL(link.Url);
            else
                curPage = WikiPage.Init(typeof(WikiDocument), new WikiParams(link.Url));
    }
}//347 - 337