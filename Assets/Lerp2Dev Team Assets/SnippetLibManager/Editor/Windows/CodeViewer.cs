using UnityEngine;
using UnityEditor;

public class CodeViewer : EditorWindow
{

    public static CodeViewer me;
    private static WikiCodeTag cont;
    public static string[] code;

    private static Vector2 scroll;
    private static Texture2D codeBackground;
    private static GUIStyleState gss;

    /*static CodeViewer()
    {
        if(code == null)
            EditorApplication.update += CloseWindow;
    }
    
    static void CloseWindow()
    {
        EditorApplication.update -= CloseWindow;
        if(me == null)
            me = GetWindow<CodeViewer>();
        me.Close();
    }*/

    private static bool showScroll
    {
        get
        {
            return code != null && code.Length > 20;
        }
    }

    public static void Init(WikiCodeTag c)
    {
        me = GetWindow<CodeViewer>();
        me.titleContent = new GUIContent("Code Viewer");
        me.minSize = new Vector2(600, 400);
        me.maxSize = new Vector2(600, 400);
        me.Show();
        codeBackground = new Color32(249, 249, 249, 255).UnitTex();
        gss = new GUIStyleState() { background = codeBackground };
        cont = c;
        code = cont.Code.Split('\n');
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Copy entire text to clipboard", GUILayout.Width(200)))
        {
            EditorGUIUtility.systemCopyBuffer = cont.RawCode;
            EditorUtility.DisplayDialog("Clipboard message", "Code copied to clipboard!", "Ok");
        }
        if (GUILayout.Button("Download to a file", GUILayout.Width(200)))
            CodeSaver.Init(cont);
        //GUILayout.Button("Copy selected text (Ctrl+C)");
        GUILayout.EndHorizontal();
        if(gss != null)
            GUILayout.TextArea("", new GUIStyle("textarea") { normal = gss, focused = gss }, GUILayout.Width(me.position.width - (showScroll ? 20 : 5)), GUILayout.Height(me.position.height - 25));
        if (me != null)
        {
            GUILayout.BeginArea(new Rect(0, 20, me.position.width, me.position.height - 20));
            scroll = GUILayout.BeginScrollView(scroll, false, showScroll, GUILayout.Height(me.position.height - 20));
        }
        GUILayout.Label("");
        if (code != null)
            foreach (string c in code) //EditorGUILayout.SelectableLabel
                GUILayout.Label(c, new GUIStyle("label") { richText = true, wordWrap = true, margin = new RectOffset(10, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0), normal = new GUIStyleState() { textColor = Color.black } }, GUILayout.Width(me.position.width - (showScroll ? 30 : 15)));
        GUILayout.Label("");
        if (me != null)
        {
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
	
}
