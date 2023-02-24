using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class AreaTrigger : MonoBehaviour {
    [SerializeField] List<string> _triggerables = new List<string>();
    [SerializeField] bool _stackable = false;
    [SerializeField] int _priority = 0;

    [Header("Debug")]
    [SerializeField] Color _debugColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField] bool _alwaysDebug = false;

    [HideInInspector, SerializeField] BoxCollider2D[] _colliders;

    public bool Stackable => _stackable;
    public int Priority => _priority;

    #region Unity Callbacks

    private void OnValidate() {
        GetColliders();
    }

    private void Awake() {
        GetColliders();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        OnEnterArea(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        OnExitArea(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        OnEnterArea(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        OnExitArea(collision.gameObject);
    }

    #endregion

    public void OnEnterArea(GameObject target) {
        EntityTriggerAreas entity = target.GetComponent<EntityTriggerAreas>();
        if (entity == null) { return; }
        if (!TriggerableBy(entity.Groups)) { return; }

        entity.EnterArea(this);
    }

    public void OnExitArea(GameObject target) {
        EntityTriggerAreas entity = target.GetComponent<EntityTriggerAreas>();
        if (entity == null) { return; }
        if (!TriggerableBy(entity.Groups)) { return; }

        entity.ExitArea(this);
    }

    public void Enter(EntityTriggerAreas entity) {
        this.Hurl("entred");
    }

    public void Exit(EntityTriggerAreas entity) {
        this.Hurl("exited");
    }

    public bool TriggerableBy(List<string> groups) {
        if (_triggerables.Count <= 0) { return true; }
        if (groups.Count <= 0) { return false; }

        for (int i = 0; i < groups.Count; i++) {
            if (_triggerables.Contains(groups[i])) {
                return true;
            }
        }
        return false;
    }

    private void GetColliders() {
        _colliders = GetComponents<BoxCollider2D>();
    }

    private void OnDrawGizmos() {
        if (!_alwaysDebug) { return; }
        Gizmos.color = _debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < _colliders.Length; i++) {
            Gizmos.DrawCube(Vector2.zero + _colliders[i].offset, _colliders[i].size);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = _debugColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < _colliders.Length; i++) {
            Gizmos.DrawCube(Vector2.zero + _colliders[i].offset, _colliders[i].size);
        }
    }
}
