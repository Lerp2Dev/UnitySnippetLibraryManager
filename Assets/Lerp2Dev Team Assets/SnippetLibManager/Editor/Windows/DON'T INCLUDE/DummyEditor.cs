using UnityEngine;
using UnityEditor;

public class DummyEditor : EditorWindow
{
    [MenuItem("Assets/Save Scene Skin")]
    static public void SaveEditorSkin()
    {
        GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene)) as GUISkin;
        AssetDatabase.CreateAsset(skin, "Assets/SceneSkin.guiskin");
    }
}