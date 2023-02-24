using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ToolsBoxEngine.BetterEvents;

namespace ToolsBoxEngine.BetterCoroutines {
    public class BetterCoroutines {
        [SerializeField, HideInInspector] BetterEvent<GameObject> _onStart = new BetterEvent<GameObject>();
        [SerializeField, HideInInspector] BetterEvent<GameObject> _onEnd = new BetterEvent<GameObject>();

        bool _working = false;
        Coroutine _routine_wrapper;
        Coroutine _routine_main;
        IEnumerator _enumerator;
        MonoBehaviour _holder;

        public bool IsWorking => _working;
        public MonoBehaviour Holder => _holder;

        public event UnityAction<GameObject> OnStart { add => _onStart += value; remove => _onStart -= value; }
        public event UnityAction<GameObject> OnEnd { add => _onEnd += value; remove => _onEnd -= value; }

        public BetterCoroutines(MonoBehaviour holder, IEnumerator enumerator = null) {
            Set(holder, enumerator);
        }

        public BetterCoroutines Set(MonoBehaviour holder, IEnumerator enumerator = null) {
            _holder = holder;
            _enumerator = enumerator;
            _routine_main = null;
            _routine_wrapper = null;
            _working = false;
            return this;
        }

        #region Start

        public void Start() {
            try {
                _routine_main = _holder.StartCoroutine(_enumerator);
                _routine_wrapper = _holder.StartCoroutine(IEWrapper(_routine_main, _holder));
            } catch (System.NullReferenceException e) {
                if (_holder == null)
                    Debug.LogError("Holder is null : " + e);
                else if (_enumerator == null)
                    Debug.LogError("IEnumerator is null : " + e);
                else
                    Debug.LogError(e);
            }
        }

        public void Start(IEnumerator enumerator) {
            _enumerator = enumerator;
            Start();
        }

        #endregion

        #region Stop

        public void Stop() {
            try {
                if (_routine_main != null) { _holder.StopCoroutine(_routine_main); }
            } catch (System.NullReferenceException e) {
                if (_holder == null)
                    Debug.LogError("Holder is null : " + e);
                else
                    Debug.LogError(e);
            }
        }

        public void AbruptStop() {
            try {
                if (_routine_wrapper != null) { _holder.StopCoroutine(_routine_wrapper); }
                _working = false;
                Stop();
            } catch (System.NullReferenceException e) {
                if (_holder == null)
                    Debug.LogError("Holder is null : " + e);
                else
                    Debug.LogError(e);
            }
        }

        #endregion

        public void ClearListeners() {
            _onStart.ClearListener();
            _onEnd.ClearListener();
        }

        public void StopAndStart() {
            Stop();
            Start();
        }

        public void StopAndStart(IEnumerator enumerator) {
            Stop();
            Start(enumerator);
        }

        IEnumerator IEWrapper(Coroutine routine, MonoBehaviour target) {
            _onStart.Invoke(target.gameObject);
            _working = true;
            yield return routine;
            _working = false;
            _onEnd.Invoke(target.gameObject);
        }
    }
}
