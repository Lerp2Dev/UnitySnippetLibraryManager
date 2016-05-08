using UnityEngine;
using UnityEditor;
using System.IO;

public class CodeSaver : EditorWindow
{

    public static CodeSaver me;
    private static WikiCodeTag cont;

    private static GUISkin skin;
    private static string m_Path;
    private static FileBrowser m_fileBrowser;
    private static Texture2D m_directoryImage,
                        m_fileImage;

    private static string m_fileName = "",
                          m_ext;

    public static void Init(WikiCodeTag c)
    {
        me = GetWindow<CodeSaver>(typeof(CodeViewer));
        me.titleContent = new GUIContent("Code Saver");
        me.minSize = new Vector2(600, 400);
        me.maxSize = new Vector2(600, 400);
        me.Show();
        cont = c;
        skin = Resources.Load<GUISkin>("Skins/Default");
        m_directoryImage = Resources.Load<Texture2D>("Textures/folder");
        m_fileImage = Resources.Load<Texture2D>("Textures/file");
        m_ext = WikiDocument.GetScriptExtension(cont.CodeType);
    }

    void OnGUI()
    {
        GUI.skin = skin;
        if(string.IsNullOrEmpty(m_Path))
            if (m_fileBrowser != null)
                m_fileBrowser.OnGUI();
            else
                OnGUIMain();
        else
        {
            GUILayout.Label(string.Format("New file '{0}' will be saved in '{1}'", m_fileName ?? "Unnamed" + m_ext, m_Path));
            GUILayout.BeginHorizontal(GUILayout.Width(me.position.width - 5));
            m_fileName = GUILayout.TextField(m_fileName);
            GUILayout.Box(m_ext, GUILayout.Width(50));
            if (GUILayout.Button("Save file", GUILayout.Width(100)))
                Save(Path.Combine(m_Path, m_fileName + m_ext));
            GUILayout.EndHorizontal();
        }
    }

    void OnGUIMain()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Folder to save the file:", GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.Label(m_Path ?? "None selected");
        if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
        {
            m_fileBrowser = new FileBrowser(
                this,
                FileBrowserType.Directory,
                "Choose directory to save code file",
                FileSelectedCallback
            );
            m_fileBrowser.DirectoryImage = m_directoryImage;
            m_fileBrowser.FileImage = m_fileImage;
        }
        GUILayout.EndHorizontal();
    }

    void FileSelectedCallback(string path)
    {
        m_fileBrowser = null;
        m_Path = path;
    }

    void Save(string path)
    {
        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
                sw.Write(cont.CCode.RawCode);
            if (EditorUtility.DisplayDialog("System dialog", "File saved successfully!", "Ok"))
            {
                AssetDatabase.Refresh();
                me.Close();
            }
        }
        else
            EditorUtility.DisplayDialog("System message", "File already exists, please use another name.", "Ok");
    }

}
