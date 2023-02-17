using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragManager : MonoBehaviour {
    static DragManager _instance;

    static GameObject _dragging;

    [SerializeField] Camera _camera;
    [SerializeField] float _z_offset = 1f;

    Vector3 _offset;
    Vector2? _selectionStart;

    private void Reset() {
        _camera = Camera.main;
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            _selectionStart = _camera.ScreenToWorldPoint(Input.mousePosition);
        } else if (Input.GetKeyUp(KeyCode.Mouse1)) {
            
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (_dragging == null) {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    Pickup(hit.collider.gameObject);
                }
            } else {
                _dragging.transform.position = new Vector3(_dragging.transform.position.x, _dragging.transform.position.y, _z_offset);
                Drop();
            }
        }

        if (_dragging != null) {
            Vector3 position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
            position.z = _z_offset;
            _dragging.transform.position = position;
        }
    }

    public static void Pickup(GameObject obj) {
        _dragging = obj;
        _instance._offset = obj.transform.position - _instance._camera.ScreenToWorldPoint(Input.mousePosition);
        _instance._offset.z = obj.transform.position.z - _instance._z_offset;
        if (obj.TryGetComponent(out IDraggable drag)) {
            drag.OnPickup();
        }
    }

    public static void Drop() {
        _dragging.transform.position = new Vector3(_dragging.transform.position.x, _dragging.transform.position.y, _instance._z_offset - _instance._offset.z);
        if (_dragging.TryGetComponent(out IDraggable drag)) {
            drag.OnDrop();
        }
        _dragging = null;
    }
}
