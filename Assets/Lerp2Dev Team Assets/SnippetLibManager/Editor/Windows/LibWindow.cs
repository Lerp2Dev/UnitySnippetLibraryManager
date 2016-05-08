using UnityEditor;
using UnityEngine;
using System.Collections;

public enum LibMenu { Wiki, Team, Repository }

[InitializeOnLoad]
public class LibWindow : EditorWindow
{
    public static LibWindow me;

    public LibMenu curMenu;
    private static Rect box;

    public static Vector2 fixedMousePos
    {
        get
        {
            return new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        }
    }

    static LibWindow()
    {
        EditorApplication.update += Init;
    }

    // Use this for initialization
    [MenuItem("Snippet/Init Library Window...")]
    static void Init()
    {
        me = GetWindow<LibWindow>(typeof(SceneView));
        me.titleContent = new GUIContent(" Snippets", Resources.Load<Texture>("Textures/code"));
        me.Show();
        EditorApplication.update -= Init;
        box = new Rect(135, 55, Screen.width - 155, Screen.height - 85);
        HtmlConverter.Init();
        if (Wiki.Initializator.Count == 0)
            Wiki.Init();
    }

    // Update is called once per frame 
    void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.ScrollWheel && Wiki.readyScr && box.Contains(e.mousePosition))
        {
            Wiki.scrScroll.y += e.delta.y;
            Repaint();
        }
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Wiki Library", "This section contains all the scripts & sippets from the Unity's Wiki."))) curMenu = LibMenu.Wiki;
        if (GUILayout.Button(new GUIContent("Team Library", "This section contains all the scripts & sippets from the Lerp2Dev's Team."))) curMenu = LibMenu.Team;
        if (GUILayout.Button(new GUIContent("Repository Library", "This section contains important recopiled repositories by the author from Github, Bitbucket, etc..."))) curMenu = LibMenu.Repository;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        if(curMenu == LibMenu.Wiki)
            Wiki.Draw();
        else
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Work in progress...");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
    }

}