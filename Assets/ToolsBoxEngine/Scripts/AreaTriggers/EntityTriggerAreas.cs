using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class EntityTriggerAreas : MonoBehaviour {
    [SerializeField] List<string> _groups = new List<string>();
    [SerializeField] SizedDictionary<List<Counted<AreaTrigger>>> _currentAreas = new SizedDictionary<List<Counted<AreaTrigger>>>();

    public List<string> Groups => _groups;

    public void EnterArea(AreaTrigger area) {
        // Déjà dans la zone
        Counted<AreaTrigger> findArea = FindArea(area);
        if (findArea != null) { ++findArea; return; }

        AreaTrigger lastArea = null;
        if (_currentAreas.Length > 0) { // Sors de la dernière Area
            lastArea = _currentAreas.Last[^1].Value;
            if (lastArea.Stackable && lastArea.Priority <= area.Priority) {
                lastArea.Exit(this);
            }
        }

        if (!_currentAreas.Contains(area.Priority)) {
            _currentAreas.Add(area.Priority, new List<Counted<AreaTrigger>>());
        }
        _currentAreas[area.Priority].Add(new Counted<AreaTrigger>(area));
        if (area.Stackable && lastArea != null && lastArea.Priority > area.Priority) { return; }
        area.Enter(this);
    }

    public void ExitArea(AreaTrigger area) {
        // Plusieurs fois dans la zone
        Counted<AreaTrigger> findArea = FindArea(area);
        if (findArea == null) { return; }
        if (findArea.Count > 0) { --findArea; return; }

        if (_currentAreas[area.Priority].Count == 1) { _currentAreas.Remove(area.Priority); } 
        else { _currentAreas[area.Priority].Remove(findArea); }

        if (area.Stackable && area.Priority < _currentAreas.Length) { return; }
        area.Exit(this);

        if (!area.Stackable) { return; }
        if (_currentAreas.Length > 0) { // Rentre dans la dernière Area
            AreaTrigger newArea = _currentAreas.Last[^1].Value;
            if (newArea.Stackable) {
                newArea.Enter(this);
            }
        }
    }

    public bool IsInArea(AreaTrigger area) {
        if (!_currentAreas.Contains(area.Priority)) { return false; }
        return FindArea(area).Value != null;
    }

    public Counted<AreaTrigger> FindArea(AreaTrigger area) {
        if (!_currentAreas.Contains(area.Priority)) { return null; }

        for (int i = 0; i < _currentAreas[area.Priority].Count; i++) {
            if (_currentAreas[area.Priority][i].Value == area) {
                return _currentAreas[area.Priority][i];
            }
        }

        return null;
    }
}

public class SizedDictionary<T> {
    Dictionary<int, T> values;
    int length = 0;

    public int Length => length;
    public T this[int i] { get { if (!Contains(i)) { return default(T); } return values[i]; } }
    public T Last => this[Length - 1];

    public SizedDictionary() {
        values = new Dictionary<int, T>();
        length = 0;
    }

    public void Add(int index, T element) {
        if (values.ContainsKey(index)) { return; }
        values.Add(index, element);
        if (index >= length) {
            length = index + 1;
        }
    }

    public void Remove(int index) {
        if (!values.ContainsKey(index)) { return; }
        values.Remove(index);
        if (index == length - 1) {
            length = ComputeLength();
        }
    }

    public bool Contains(int key) {
        return values.ContainsKey(key);
    }

    public int ComputeLength() {
        int max = 0;
        foreach (var key in values.Keys) {
            if (key >= max) {
                max = key + 1;
            }
        }
        return max;
    }

    public override string ToString() {
        string output = "";
        foreach (var item in values) {
            output += item.Key + ":" + item.Value + "; ";
        }
        return output;
    }
}