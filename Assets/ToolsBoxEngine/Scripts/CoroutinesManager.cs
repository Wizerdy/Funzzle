using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutinesManager : MonoBehaviour {
    #region Instance

    static CoroutinesManager _instance;

    public static CoroutinesManager Instance { get { if (_instance == null) { CreateInstance(); } return _instance; } }

    private static void CreateInstance() {
        if (_instance != null) { return; }
        GameObject obj = new GameObject("CoroutinesManager");
        _instance = obj.AddComponent<CoroutinesManager>();
    }

    #endregion

    private void Start() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    public static Coroutine Start(IEnumerator routine) {
        return Instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine routine) {
        if (_instance == null) { return; }
        _instance.StopCoroutine(routine);
    }
}
