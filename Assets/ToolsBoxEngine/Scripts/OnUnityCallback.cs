using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToolsBoxEngine {
    public class OnUnityCallback : MonoBehaviour {
        [System.Serializable]
        enum UnityCallback { AWAKE, ENABLE, START, UPDATE, DISABLE, DESTROY, COLLISION_ENTER, TRIGGER_ENTER }

        [SerializeField] UnityCallback _callback;
        [SerializeField] float _delay = 0f;
        [Space]
        [SerializeField] UnityEvent<GameObject> _action;

        void Awake() {
            if (_callback == UnityCallback.AWAKE) { Invoke(); }
        }

        void OnEnable() {
            if (_callback == UnityCallback.ENABLE) { Invoke(); }
        }

        void Start() {
            if (_callback == UnityCallback.START) { Invoke(); }
        }

        void Update() {
            if (_callback == UnityCallback.UPDATE) { Invoke(); }
        }

        void OnDisable() {
            if (_callback == UnityCallback.DISABLE) { Invoke(); }
        }

        void OnDestroy() {
            if (_callback == UnityCallback.DESTROY) { Invoke(); }
        }

        private void OnCollisionEnter(Collision collision) {
            if (_callback == UnityCallback.COLLISION_ENTER) { Invoke(); }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (_callback == UnityCallback.COLLISION_ENTER) { Invoke(); }
        }

        private void OnTriggerEnter(Collider collision) {
            if (_callback == UnityCallback.TRIGGER_ENTER) { Invoke(); }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (_callback == UnityCallback.TRIGGER_ENTER) { Invoke(); }
        }

        private void Invoke() {
            if (_delay > 0f) { StartCoroutine(Tools.Delay(() => _action?.Invoke(gameObject), _delay)); }
            else { _action?.Invoke(gameObject); }
        }
    }
}