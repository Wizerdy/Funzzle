using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ToolsBoxEngine;

public class TagSelectorAttribute : PropertyAttribute {
    public bool useDefaultTagFieldDrawer = false;

    public TagSelectorAttribute(bool useDefaultTagFieldDrawer = false) {
        this.useDefaultTagFieldDrawer = useDefaultTagFieldDrawer;
    }
}

[System.Serializable]
public class MultipleTagSelector {
    public enum State { NONE, MULTIPLE, EVERYTHING }

    public List<string> tags;
    public State state = State.MULTIPLE;

    //static private readonly MultipleTagSelector _none = new MultipleTagSelector(State.NONE);
    //static private readonly MultipleTagSelector _everything = new MultipleTagSelector(State.EVERYTHING);

    //static public MultipleTagSelector Everything => _everything;
    //static public MultipleTagSelector None => _none;

    public MultipleTagSelector(State state = State.MULTIPLE) {
        this.state = state;
        tags = new List<string>();
    }

    public bool Contains(string tag) {
        if (state == State.EVERYTHING) { return true; }
        if (state == State.NONE) { return false; }
        return tags.Contains(tag);
    }
}