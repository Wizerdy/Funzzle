using System.Collections;
using System.Collections.Generic;
using ToolsBoxEngine;
using Unity.VisualScripting;
using UnityEngine;

public class DragManager : MonoBehaviour {
    static DragManager _instance;

    static GameObject _dragging;

    [SerializeField] Camera _camera;
    [SerializeField] float _z_offset = 1f;
    [SerializeField] LayerMask _raycastMask;

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

                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.black, 5f);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f, _raycastMask)) {
                    Pickup(hit.collider.gameObject.GetComponentInRoot<IDraggable>());
                }
            } else {
                //_dragging.transform.position = new Vector3(_dragging.transform.position.x, _dragging.transform.position.y, _z_offset);
                Drop();
            }
        }

        if (_dragging != null) {
            //Vector3 position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
            Vector3 position = MouseToWorldPosition(_camera, _offset.z) + _offset.Override(0f, Axis.Z);
            //position.z = _z_offset;
            _dragging.transform.position = position;
        }
    }

    private static Vector3 MouseToWorldPosition(Camera camera, float z) {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, z));
        plane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    public static void Pickup(IDraggable obj) {
        if (obj == null) { return; }

        _dragging = obj.GameObject;
        _instance._offset = obj.GameObject.transform.position - MouseToWorldPosition(_instance._camera, _dragging.transform.position.z + _instance._z_offset);
        _instance._offset.z = _dragging.transform.position.z + _instance._z_offset;

        if (_dragging.TryGetRootComponent(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        obj.OnPickup();
    }

    public static void Drop() {
        if (_dragging.TryGetRootComponent(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
        }

        if (_dragging.TryGetRootComponent(out IDraggable drag)) {
            drag.OnDrop();
        }
        _dragging = null;
    }
}
