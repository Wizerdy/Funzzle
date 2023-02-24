//using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class ReferenceWindow : EditorWindow {
    //[SerializeField] MonoScript script;
    const string BASE_PATH = "/_Project/Scripts/References";

    [SerializeField] string path = "";
    [SerializeField] string shortPath = "";
    [SerializeField] string className;
    [SerializeField] string oldClassName;
    bool success = false;

    [MenuItem("Tools/Reference Creater")]
    public static void ShowWindow() {
        EditorWindow window = EditorWindow.GetWindow(typeof(ReferenceWindow));
        window.minSize = new Vector2(200f, 60f);
        window.maxSize = new Vector2(600f, 60f);
    }

    public void Awake() {
        success = false;
        className = "";
        path = Application.dataPath + BASE_PATH;
        ComputeShortPath();
    }

    private void OnGUI() {
        if (GUILayout.Button("Path : " + shortPath)) {
            PathPanel();
        }
        className = EditorGUILayout.TextField(className);
        if (oldClassName != className) {
            success = false;
        }

        if (className.Equals("")) { GUI.enabled = false; }
        if (GUILayout.Button("Create Class Reference")) {
            success = CreateReference(className, path);
        }
        GUI.enabled = true;
        if (success) { GUILayout.Label(className + " Reference created"); }
        oldClassName = className;
    }

    private void PathPanel() {
        path = EditorUtility.OpenFolderPanel("Path", Application.dataPath + BASE_PATH, "");
        if (path.Equals("")) { path = Application.dataPath + BASE_PATH; }
        ComputeShortPath();
    }

    private void ComputeShortPath() {
        if (Application.dataPath.Length >= path.Length) { shortPath = ""; return; }
        shortPath = path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length);
    }

    private bool CreateReference<T>(T script) {
        if (script == null) { return false; }

        string type = typeof(T).ToString();
        return CreateReference(type);
    }

    private bool CreateReference(string className, string path) {
        if (className.Equals("")) { return false; }

        string type = className;

        string text = GetUsings() +
            $"[CreateAssetMenu(menuName = \"Reference/{type}\")]\n" +
            $"public class {type}Reference : Reference<{type}>" + " {\n\n}";

        CreateAsset(path, $"{type}Reference", text);

        text = GetUsings() +
            $"public class {type}ReferenceSetter : ReferenceSetter<{type}>" + " {\n\n}";

        CreateAsset(path, $"{type}ReferenceSetter", text);

        AssetDatabase.Refresh();
        return true;
    }

    private void CreateAsset(string path, string name, string content) {
        using (StreamWriter sw = new StreamWriter(string.Format(path + "/{0}.cs", name))) {
            sw.Write(content);
        }
    }

    private string GetUsings() {
        return "using System.Collections;\n" +
            "using System.Collections.Generic;\n" +
            "using UnityEngine;\n" +
            "\n";
    }
}
