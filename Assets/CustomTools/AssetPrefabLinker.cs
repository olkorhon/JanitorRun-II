
#if (UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class AssetPrefabLinker : EditorWindow
{
    string matchedString = "";
    Regex gameObjectMatcher;
    List<GameObject> matchingGameObjects = new List<GameObject>();
    GUIStyle resultLabelStyleGameobject = new GUIStyle();

    string prefabString = "";
    Regex prefabMatcher;
    List<string> matchingPrefabFiles = new List<string>();
    GameObject selectedPrefab;
    GUIStyle resultLabelStylePrefab = new GUIStyle();


	[MenuItem("Window/PrefabLinker")]
    public static void ShowWindow()
    {
        GetWindow<AssetPrefabLinker>("Prefab Linker");
    }

    // Update is called once per frame
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Target gameobject regex");
        this.matchedString = GUI.TextField(new Rect(15, 30, 200, 20), this.matchedString, 25);
        showSearchResultLabel();

        GUI.Label(new Rect(10, 80, 200, 20), "Desired prefab");
        this.prefabString = GUI.TextField(new Rect(15, 100, 200, 20), this.prefabString, 25);
        showPrefabSearchStatus();

        if (GUI.changed)
        {
            updateGameObjectRegex(this.matchedString);
            updatePrefabRegex(this.prefabString);
        }

        if (matchingGameObjects.Count > 0 && selectedPrefab != null)
            executeButton();
    }

    private void updateGameObjectRegex(string pattern)
    {
        this.gameObjectMatcher = new Regex(pattern);

        matchingGameObjects.Clear();
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in gameObjects)
        {
            if (this.gameObjectMatcher.IsMatch(go.name) == true)
            {
                matchingGameObjects.Add(go);
            }
        }
    }

    private void updatePrefabRegex(string pattern)
    {
        this.prefabMatcher = new Regex(pattern);
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();

        matchingPrefabFiles.Clear();
        foreach (string path in assetPaths)
            if (path.EndsWith(".prefab") && this.prefabMatcher.IsMatch(path))
                matchingPrefabFiles.Add(path);

        if (matchingPrefabFiles.Count == 1)
            selectedPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(matchingPrefabFiles[0], typeof(GameObject));
        else
            selectedPrefab = null;
    }

    private void showSearchResultLabel()
    {
        if (matchingGameObjects.Count > 0)
        {
            resultLabelStyleGameobject.normal.textColor = new Color(0f, 0.5f, 0f);
            GUI.Label(new Rect(15, 52, 200, 20), matchingGameObjects.Count + " matches", resultLabelStyleGameobject);
        }
        else
        {
            resultLabelStyleGameobject.normal.textColor = Color.red;
            GUI.Label(new Rect(15, 52, 200, 20), "No results", resultLabelStyleGameobject);
        }
    }

    private void showPrefabSearchStatus()
    {
        if (selectedPrefab != null)
        {
            resultLabelStylePrefab.normal.textColor = new Color(0f, 0.5f, 0f);
            GUI.Label(new Rect(15, 122, 200, 20), "Prefab found: " + selectedPrefab.name, resultLabelStylePrefab);
        }
        else if (matchingPrefabFiles.Count > 1)
        {
            resultLabelStylePrefab.normal.textColor = new Color(0.5f, 0.5f, 0f);
            GUI.Label(new Rect(15, 122, 200, 20), "More than one match, please specify", resultLabelStylePrefab);
        }
        else
        {
            resultLabelStylePrefab.normal.textColor = Color.red;
            GUI.Label(new Rect(15, 122, 200, 20), "No matches", resultLabelStylePrefab);
        }
    }

    private void executeButton()
    {
        if (GUI.Button(new Rect(10, 162, 100, 20), "Execute"))
        {
            foreach (GameObject originalObject in this.matchingGameObjects)
            {
                GameObject newThing = Instantiate(selectedPrefab);

                newThing.name = originalObject.name;
                newThing.transform.position = originalObject.transform.position;
                newThing.transform.rotation = originalObject.transform.rotation;

                newThing.transform.SetParent(originalObject.transform.parent, true);
                DestroyImmediate(originalObject);
            }

            updateGameObjectRegex(this.matchedString);
        }
    }
}

#endif