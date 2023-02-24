using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class StaticAxis : MonoBehaviour {
    enum PositionReference { WORLD, LOCAL }

    [SerializeField] Axis _frozenAxis;
    [SerializeField] PositionReference _positionReference;
    [SerializeField] float _value;

    void Update() {
        switch (_positionReference) {
            case PositionReference.WORLD:
                transform.position = transform.position.Override(_value, _frozenAxis);
                break;
            case PositionReference.LOCAL:
                transform.localPosition = transform.localPosition.Override(_value, _frozenAxis);
                break;
        }
    }
}
