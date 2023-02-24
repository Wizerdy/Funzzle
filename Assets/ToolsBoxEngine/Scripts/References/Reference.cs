using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reference<T> : ScriptableObject, IReferenceSetter<T>, IValid where T : class {
    [SerializeField] T _instance;

    public T Instance => _instance;
    public bool IsValid => _instance != null;

    void IReferenceSetter<T>.SetInstance(T newInstance) {
        _instance = newInstance;
    }
}

public static class ReferenceTools {
    public static bool Valid(this IValid valid) {
        if (valid != null && !valid.IsValid) { Debug.LogWarning("Reference not set : " + valid.GetType().ToString()); }
        return valid != null && valid.IsValid;
    }
}
