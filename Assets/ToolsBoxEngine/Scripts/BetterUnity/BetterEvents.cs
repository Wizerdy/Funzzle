using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToolsBoxEngine.BetterEvents {
    [Serializable]
    public class BetterEvent<T1, T2, T3, T4> {
        [Space]
        [SerializeField] UnityEvent<T1, T2, T3, T4> _event;

        public event UnityAction<T1, T2, T3, T4> Event { add => _event.AddListener(value); remove => _event.RemoveListener(value); }

        public BetterEvent() {
            _event = new UnityEvent<T1, T2, T3, T4>();
        }

        public void Invoke(T1 argument1, T2 argument2, T3 argument3, T4 argument4) {
            try {
                _event?.Invoke(argument1, argument2, argument3, argument4);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AddListener(UnityAction<T1, T2, T3, T4> action) {
            _event.AddListener(action);
        }

        public void RemoveListener(UnityAction<T1, T2, T3, T4> action) {
            _event.RemoveListener(action);
        }

        public void ClearListener() {
            _event.RemoveAllListeners();
        }

        public static BetterEvent<T1, T2, T3, T4> operator +(BetterEvent<T1, T2, T3, T4> e, UnityAction<T1, T2, T3, T4> action) {
            e.AddListener(action);
            return e;
        }

        public static BetterEvent<T1, T2, T3, T4> operator +(BetterEvent<T1, T2, T3, T4> e, BetterEvent<T1, T2, T3, T4> action) {
            e.AddListener(action.Invoke);
            return e;
        }

        public static BetterEvent<T1, T2, T3, T4> operator -(BetterEvent<T1, T2, T3, T4> e, UnityAction<T1, T2, T3, T4> action) {
            e.RemoveListener(action);
            return e;
        }

        public static BetterEvent<T1, T2, T3, T4> operator -(BetterEvent<T1, T2, T3, T4> e, BetterEvent<T1, T2, T3, T4> action) {
            e.RemoveListener(action.Invoke);
            return e;
        }
    }

    [Serializable]
    public class BetterEvent<T1, T2, T3> {
        [Space]
        [SerializeField] UnityEvent<T1, T2, T3> _event;

        public event UnityAction<T1, T2, T3> Event { add => _event.AddListener(value); remove => _event.RemoveListener(value); }

        public BetterEvent() {
            _event = new UnityEvent<T1, T2, T3>();
        }

        public void Invoke(T1 argument1, T2 argument2, T3 argument3) {
            try {
                _event?.Invoke(argument1, argument2, argument3);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AddListener(UnityAction<T1, T2, T3> action) {
            _event.AddListener(action);
        }

        public void RemoveListener(UnityAction<T1, T2, T3> action) {
            _event.RemoveListener(action);
        }

        public void ClearListener() {
            _event.RemoveAllListeners();
        }

        public static BetterEvent<T1, T2, T3> operator +(BetterEvent<T1, T2, T3> e, UnityAction<T1, T2, T3> action) {
            e.AddListener(action);
            return e;
        }

        public static BetterEvent<T1, T2, T3> operator +(BetterEvent<T1, T2, T3> e, BetterEvent<T1, T2, T3> action) {
            e.AddListener(action.Invoke);
            return e;
        }

        public static BetterEvent<T1, T2, T3> operator -(BetterEvent<T1, T2, T3> e, UnityAction<T1, T2, T3> action) {
            e.RemoveListener(action);
            return e;
        }

        public static BetterEvent<T1, T2, T3> operator -(BetterEvent<T1, T2, T3> e, BetterEvent<T1, T2, T3> action) {
            e.RemoveListener(action.Invoke);
            return e;
        }
    }

    [Serializable]
    public class BetterEvent<T1, T2> {
        [Space]
        [SerializeField] UnityEvent<T1, T2> _event;

        public event UnityAction<T1, T2> Event { add => _event.AddListener(value); remove => _event.RemoveListener(value); }

        public BetterEvent() {
            _event = new UnityEvent<T1, T2>();
        }

        public void Invoke(T1 argument1, T2 argument2) {
            try {
                _event?.Invoke(argument1, argument2);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AddListener(UnityAction<T1, T2> action) {
            _event.AddListener(action);
        }

        public void RemoveListener(UnityAction<T1, T2> action) {
            _event.RemoveListener(action);
        }

        public void ClearListener() {
            _event.RemoveAllListeners();
        }

        public static BetterEvent<T1, T2> operator +(BetterEvent<T1, T2> e, UnityAction<T1, T2> action) {
            e.AddListener(action);
            return e;
        }

        public static BetterEvent<T1, T2> operator +(BetterEvent<T1, T2> e, BetterEvent<T1, T2> action) {
            e.AddListener(action.Invoke);
            return e;
        }

        public static BetterEvent<T1, T2> operator -(BetterEvent<T1, T2> e, UnityAction<T1, T2> action) {
            e.RemoveListener(action);
            return e;
        }

        public static BetterEvent<T1, T2> operator -(BetterEvent<T1, T2> e, BetterEvent<T1, T2> action) {
            e.RemoveListener(action.Invoke);
            return e;
        }
    }

    [Serializable]
    public class BetterEvent<T> {
        [Space]
        [SerializeField] UnityEvent<T> _event;

        public event UnityAction<T> Event { add => _event.AddListener(value); remove => _event.RemoveListener(value); }

        public BetterEvent() {
            _event = new UnityEvent<T>();
        }

        public void Invoke(T argument) {
            try {
                _event?.Invoke(argument);
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AddListener(UnityAction<T> action) {
            _event.AddListener(action);
        }

        public void RemoveListener(UnityAction<T> action) {
            _event.RemoveListener(action);
        }

        public void ClearListener() {
            _event.RemoveAllListeners();
        }

        public static BetterEvent<T> operator +(BetterEvent<T> e, UnityAction<T> action) {
            e.AddListener(action);
            return e;
        }

        public static BetterEvent<T> operator +(BetterEvent<T> e, BetterEvent<T> action) {
            e.AddListener(action.Invoke);
            return e;
        }

        public static BetterEvent<T> operator -(BetterEvent<T> e, UnityAction<T> action) {
            e.RemoveListener(action);
            return e;
        }

        public static BetterEvent<T> operator -(BetterEvent<T> e, BetterEvent<T> action) {
            e.RemoveListener(action.Invoke);
            return e;
        }
    }

    [Serializable]
    public class BetterEvent {
        [Space]
        [SerializeField] UnityEvent _event;

        public event UnityAction Event { add => _event.AddListener(value); remove => _event.RemoveListener(value); }

        public BetterEvent() {
            _event = new UnityEvent();
        }

        public void Invoke() {
            try {
                _event?.Invoke();
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        public void AddListener(UnityAction action) {
            _event.AddListener(action);
        }

        public void RemoveListener(UnityAction action) {
            _event.RemoveListener(action);
        }

        public void ClearListener() {
            _event.RemoveAllListeners();
        }

        public static BetterEvent operator +(BetterEvent e, UnityAction action) {
            e.AddListener(action);
            return e;
        }

        public static BetterEvent operator +(BetterEvent e, BetterEvent action) {
            e.AddListener(action.Invoke);
            return e;
        }

        public static BetterEvent operator -(BetterEvent e, UnityAction action) {
            e.RemoveListener(action);
            return e;
        }

        public static BetterEvent operator -(BetterEvent e, BetterEvent action) {
            e.RemoveListener(action.Invoke);
            return e;
        }
    }

    public class StackableFunc<T> {
        public List<Func<T, T>> _funcs;

        public int Count => _funcs.Count;

        #region Big Five

        public StackableFunc() {
            _funcs = new List<Func<T, T>>();
        }

        public StackableFunc(StackableFunc<T> input) {
            _funcs = new List<Func<T, T>>(input._funcs);
        }

        public static StackableFunc<T> operator +(StackableFunc<T> input, Func<T, T> add) {
            StackableFunc<T> output = new StackableFunc<T>(input);
            output.Add(add);
            return output;
        }

        public static StackableFunc<T> operator -(StackableFunc<T> input, Func<T, T> add) {
            StackableFunc<T> output = new StackableFunc<T>(input);
            output.Remove(add);
            return output;
        }

        #endregion

        public T Use(T arg) {
            for (int i = 0; i < _funcs.Count; i++) {
                arg = _funcs[i](arg);
            }
            return arg;
        }

        #region Lists Functions

        public void Remove(Func<T, T> func) {
            _funcs.Remove(func);
        }

        public void Clear() {
            _funcs.Clear();
        }

        public void Add(Func<T, T> func) {
            _funcs.Add(func);
        }

        public void AddFirst(Func<T, T> func) {
            _funcs.Insert(0, func);
        }

        public bool Contains(Func<T, T> func) {
            return _funcs.Contains(func);
        }

        public void Copy(StackableFunc<T> stFunc) {
            _funcs = new List<Func<T, T>>(stFunc._funcs);
        }

        public void CopyReference(StackableFunc<T> stFunc) {
            _funcs = stFunc._funcs;
        }

        #endregion
    }
}