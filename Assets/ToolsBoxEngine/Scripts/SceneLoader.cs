using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    static Dictionary<int, AsyncOperation> _asyncScenesByIndex = new Dictionary<int, AsyncOperation>();
    static Dictionary<string, AsyncOperation> _asyncScenesByName = new Dictionary<string, AsyncOperation>();

    public static void LoadScene(int index) {
        SceneManager.LoadScene(index);
    }

    public static void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public static void LoadSceneAsync(int index) {
        if (_asyncScenesByIndex.ContainsKey(index)) { return; }

        AsyncOperation scene = SceneManager.LoadSceneAsync(index);
        scene.allowSceneActivation = false;
        _asyncScenesByIndex.Add(index, scene);
    }

    public static void LoadSceneAsync(string name) {
        if (_asyncScenesByName.ContainsKey(name)) { return; }

        AsyncOperation scene = SceneManager.LoadSceneAsync(name);
        scene.allowSceneActivation = false;
        _asyncScenesByName.Add(name, scene);
    }

    public static void UseSceneAsync(int index) {
        if (!_asyncScenesByIndex.ContainsKey(index)) { return; }
        _asyncScenesByIndex[index].allowSceneActivation = true;
        _asyncScenesByIndex.Remove(index);
    }

    public static void UseSceneAsync(string name) {
        if (!_asyncScenesByName.ContainsKey(name)) { return; }
        _asyncScenesByName[name].allowSceneActivation = true;
        _asyncScenesByName.Remove(name);
    }
}
