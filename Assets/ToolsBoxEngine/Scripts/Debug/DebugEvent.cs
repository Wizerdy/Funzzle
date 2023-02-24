using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEvent : MonoBehaviour {
    public void Print(string debugText) {
        Debug.Log(debugText);
    }

    public void PrintWarning(string debugText) {
        Debug.LogWarning(debugText);
    }
    
    public void PrintError(string debugText) {
        Debug.LogError(debugText);
    }

    public void Break() {
        Debug.Break();
    }
}
