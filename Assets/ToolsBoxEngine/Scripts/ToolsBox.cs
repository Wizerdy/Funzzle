using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToolsBoxEngine {
    #region Enums

    public enum Direction { UP, RIGHT, DOWN, LEFT }
    public enum Axis { X, Y, Z, W }
    public enum Axis2D { X, Y }
    public enum DebugType { NORMAL, WARNING, ERROR }

    public enum Comparison { EQUAL, DIFFERENT, LESS, LESS_EQUAL, GREATER, GREATER_EQUAL }
    public enum BasicComparison { LESS, GREATER }

    public enum LogicGate { AND, OR, NOR, XOR, NOT }

    public enum ColliderGate { ANY, HARD, TRIGGER_HARD, TRIGGER_TRIGGER, TRIGGER }

    #endregion

    #region Classes

    public class Nullable<T> where T : struct {
        public T? value;

        public static implicit operator T(Nullable<T> a) => (T)a.value;
        public static implicit operator Nullable<T>(T a) => new Nullable<T>((T?)a);

        public Nullable() {
            value = null;
        }

        public Nullable(T? value) {
            this.value = value;
        }

        public T Value {
            get { return (T)value; }
            set { this.value = value; }
        }
    }

    [Serializable]
    public class AmplitudeCurve {
        public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float duration = 1f;
        public float amplitude = 1f;

        float timer;

        public float Timer => timer;
        public float Percentage => Mathf.Clamp01(timer / duration);

        #region Constructeurs

        public AmplitudeCurve(AnimationCurve curve, float duration, float timer, float amplitude) {
            this.curve = curve;
            this.duration = duration;
            this.timer = timer;
            this.amplitude = amplitude;
        }

        public AmplitudeCurve(AnimationCurve curve) : this(curve, 1f, 0f, 1f) { }

        public AmplitudeCurve() : this(AnimationCurve.Linear(0f, 0f, 1f, 1f)) { }

        public AmplitudeCurve Clone() {
            return new AmplitudeCurve(curve, duration, timer, amplitude);
        }

        #endregion

        public float Evaluate() {
            float ratio = Percentage;
            ratio = curve.Evaluate(ratio);
            return ratio;
        }

        public void UpdateTimer(float deltaTime) {
            timer += deltaTime;
            timer = Mathf.Clamp(timer, 0f, duration);
        }

        public void Reset() {
            timer = 0f;
        }

        public void SetPercentage(float percentage) {
            percentage = Mathf.Clamp01(percentage);
            timer = percentage * duration;
        }

        public float Evaluate(float percentage) {
            return curve.Evaluate(percentage);
        }
    }

    [System.Serializable]
    public class Counted<T> {
        T _value;
        int _count;

        public T Value { get => _value; set => _value = value; }
        public int Count { get => _count; set => _count = value; }

        public Counted(T value) {
            _value = value;
        }

        public Counted(int count, T value) {
            _count = count;
            _value = value;
        }

        public void Reset() {
            _count = 0;
        }

        public static implicit operator T(Counted<T> a) {
            return a._value;
        }

        public static implicit operator int(Counted<T> a) {
            return a._count;
        }

        public static Counted<T> operator ++(Counted<T> a) {
            ++a._count;
            return a;
        }

        public static Counted<T> operator --(Counted<T> a) {
            --a._count;
            return a;
        }
    }

    [Serializable]
    public struct Named<T> {
        public string name;
        public T value;

        public Named(T value, string name) {
            this.value = value;
            this.name = name;
        }
    }

    [Serializable]
    public class Token {
        int _token = 0;
        [HideInInspector] UnityEvent _onEmpty = new UnityEvent();
        [HideInInspector] UnityEvent _onFill = new UnityEvent();
        [HideInInspector] UnityEvent<int> _onEarn = new UnityEvent<int>();
        [HideInInspector] UnityEvent<int> _onLose = new UnityEvent<int>();

        public int Tokens => _token;

        #region Events

        public event UnityAction OnEmpty { add => _onEmpty.AddListener(value); remove => _onEmpty.RemoveListener(value); }
        public event UnityAction OnFill { add => _onFill.AddListener(value); remove => _onFill.RemoveListener(value); }
        public event UnityAction<int> OnEarn { add => _onEarn.AddListener(value); remove => _onEarn.RemoveListener(value); }
        public event UnityAction<int> OnLose { add => _onLose.AddListener(value); remove => _onLose.RemoveListener(value); }

        #endregion

        public bool HasToken => _token > 0;

        public Token() { }

        public Token(int number) {
            _token = number;
        }

        /// <summary>
        /// Add Token (True++ / False--)
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public void AddToken(bool value) {
            AddToken(value ? 1 : -1);
        }

        public void AddToken(int amount) {
            if (amount == 0) { return; }
            if (amount < 0 && _token == 0) { return; }

            if (amount > 0) { _onEarn?.Invoke(amount); }
            if (amount < 0) { _onLose?.Invoke(amount); }

            if (_token == 0) { _onFill?.Invoke(); }
            _token += amount;
            Mathf.Max(0, _token);
            if (_token == 0) { _onEmpty?.Invoke(); }
        }

        public void Reset() {
            _token = 0;
        }
    }

    #endregion

    public static class Tools {
        #region Variables
        static string[] _hurlable = {
            "Hello world ?",
            "Gino", "HERE !", "HURL", "Yellow submarine", "Did you see ?!",
            "REEEEEEEX !", "Imotep", "Ninja !", "Urgh", "Oh la belle bleue", "Ooba ooba !",
            "404", "You're teapot", "Pouet", "Bogo sorted", "Hey listen", "Pourquoi ?",
            "Mr.Gé1enormSecs", "Roblox36", "Achtung", "SirLynixVanFrietjes", "1337", "35383773",
            "Chaussette", "Avez-vous déjà vous ?", "2319", "Viva l'algérie", "C'est une menace ?",
            "Km²/h", "Skribbl ?", "Tools.Print", "Debug.Log", "HURLABLE !", "Malibu coco ?",
            "Papillon de lumière", "How dare u ?", "THIS IS C# !", "On s'en bat les couilles",
            "Connard", "(tousse)", "J'ai mal", "Elle est bonne", "Tamer", "Wingardium", "Wait",
            "C'est ma b*te", "Do a barell roll", "Titre", "Mundo go this way", "Ok", "Enormimus !",
            "Kowabunga", "Ptdr t ki ?", "Pasteque !", "Hein ?", "...", "You really need me ?",
            "T'en veux encore ?", "AÏE", "OUÏLLE", "(Adrien's noise)", "Moule", "Dublin, duh", "DUH",
            "Tartes à la framboise ?", "Chibrons", "UwU", "<color=red>ERRO-</color> nah joking",
            "Papush", "T'as fetch ?", "Derrière toi !", "Phillipe !", "Fifo le fifou", "Va sucer un ours",
            "Mange tes morts", "Peepoodo", "Peekaboo", "Lapinue", "+1", "Apple suxx", "Hit Billy & drink milk",
            "THIIIICK", "Hippity hoppity", "Scoobi-dooby-doo", "Sluuuurp", "Leave me alone", "F*CK YOU !",
            "Leblanc", "2b || !2b", "Boob", "Bark", "Fus Roh Dah", "Push Roh Dah !", "Git Rekt", "Merge tes morts",
            "Bring me egg", "Patatozilla", "La bougie.. M*rde !",
            "Goodbye world"
        };
        #endregion

        #region Delegates

        public delegate void BasicDelegate();

        public delegate void BasicDelegate<T>(T arg);

        //public delegate void BasicDelegateTwoArgs<T>(ref T arg1, T arg2);

        public delegate void BasicDelegate<T1, T2>(T1 arg1, T2 arg2);

        public delegate void BasicDelegate<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

        public delegate T BasicDelegateReturn<T>();

        public delegate T BasicDelegateReturnArg<T>(T arg);

        public delegate T2 BasicDelegateReturn<T1, T2>(T1 arg);

        #endregion

        #region Extensions methods

        #region Vectors

        #region Vector2

        public static Vector3 ToV3(this Vector2 vector, float value = 0f, Axis axis = Axis.Z) {
            switch (axis) {
                case Axis.X:
                    return new Vector3(value, vector.x, vector.y);
                case Axis.Y:
                    return new Vector3(vector.x, value, vector.y);
                case Axis.Z:
                    return new Vector3(vector.x, vector.y, value);
                default:
                    return new Vector3(vector.x, vector.y, value);
            }
        }

        public static Vector2Int ToV2I(this Vector2 vector) {
            return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        }

        public static Vector2 Override(this Vector2 vector, float value, Axis axis = Axis.Y) {
            switch (axis) {
                case Axis.X:
                    vector.x = value;
                    break;
                case Axis.Y:
                    vector.y = value;
                    break;
                case Axis.Z:
                    Debug.LogWarning("Can't override Vector2 z axis, using default axis : y");
                    vector.y = value;
                    break;
                default:
                    vector.y = value;
                    break;
            }

            return vector;
        }

        public static Vector2 Override(this Vector2 vector, Vector2 target, params Axis[] axis) {
            if (axis.Length <= 0) { return vector; }
            for (int i = 0; i < axis.Length; i++) {
                switch (axis[i]) {
                    case Axis.X:
                        vector.Override(target.x, Axis.X);
                        break;
                    case Axis.Y:
                        vector.Override(target.y, Axis.Y);
                        break;
                }
            }
            return vector;
        }

        public static Vector2 Abs(this Vector2 vector) {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector2 UpIsDown(this Vector2 vector) {
            return new Vector2(vector.x, -vector.y);
        }

        #endregion

        #region Vector2Int

        public static Vector3Int ToV3I(this Vector2Int vector, int value = 0, Axis axis = Axis.Z) {
            switch (axis) {
                case Axis.X:
                    return new Vector3Int(value, vector.x, vector.y);
                case Axis.Y:
                    return new Vector3Int(vector.x, value, vector.y);
                case Axis.Z:
                    return new Vector3Int(vector.x, vector.y, value);
                default:
                    return new Vector3Int(vector.x, vector.y, value);
            }
        }

        public static Vector2Int UpIsDown(this Vector2Int vector) {
            return new Vector2Int(vector.x, -vector.y);
        }

        #endregion

        #region Vector3

        public static Vector2 ToV2(this Vector3 vector, Axis axisToIgnore = Axis.Z) {
            switch (axisToIgnore) {
                case Axis.X:
                    return new Vector2(vector.y, vector.z);
                case Axis.Y:
                    return new Vector2(vector.x, vector.z);
                case Axis.Z:
                    return new Vector2(vector.x, vector.y);
                default:
                    return new Vector2(vector.x, vector.y);
            }
        }

        public static Vector3 Override(this Vector3 vector, float value, Axis axis = Axis.Y) {
            switch (axis) {
                case Axis.X:
                    vector.x = value;
                    break;
                case Axis.Y:
                    vector.y = value;
                    break;
                case Axis.Z:
                    vector.z = value;
                    break;
                default:
                    vector.y = value;
                    break;
            }

            return vector;
        }

        public static Vector3 Override(this Vector3 vector, Vector3 target, params Axis[] axis) {
            if (axis.Length <= 0) { return vector; }
            for (int i = 0; i < axis.Length; i++) {
                switch (axis[i]) {
                    case Axis.X:
                        vector = vector.Override(target.x, Axis.X);
                        break;
                    case Axis.Y:
                        vector = vector.Override(target.y, Axis.Y);
                        break;
                    case Axis.Z:
                        vector = vector.Override(target.z, Axis.Z);
                        break;
                }
            }
            return vector;
        }

        public static Vector3 Abs(this Vector3 vector) {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector3 Positive(this Vector3 vector) {
            return new Vector3(vector.x.Positive(), vector.y.Positive(), vector.z.Positive());
        }

        public static Vector3 Positive(this Vector3 vector, Axis axis) {
            switch (axis) {
                case Axis.X:
                    vector.x = vector.x.Positive();
                    break;
                case Axis.Y:
                    vector.y = vector.y.Positive();
                    break;
                case Axis.Z:
                    vector.z = vector.z.Positive();
                    break;
            }
            return vector;
        }

        public static Vector3 MultiplyIndividually(this Vector3 vector1, Vector3 vector2) {
            return new Vector3(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z);
        }

        #endregion

        public static Vector2Int ToV2I(this Vector3Int vector, Axis axisToIgnore = Axis.Z) {
            switch (axisToIgnore) {
                case Axis.X:
                    return new Vector2Int(vector.y, vector.z);
                case Axis.Y:
                    return new Vector2Int(vector.x, vector.z);
                case Axis.Z:
                    return new Vector2Int(vector.x, vector.y);
                default:
                    return new Vector2Int(vector.x, vector.y);
            }
        }

        public static Vector3 ToV3(this Vector4 vector, Axis axisToIgnore = Axis.W) {
            switch (axisToIgnore) {
                case Axis.X:
                    return new Vector3(vector.y, vector.z, vector.w);
                case Axis.Y:
                    return new Vector3(vector.x, vector.z, vector.w);
                case Axis.Z:
                    return new Vector3(vector.x, vector.y, vector.w);
                case Axis.W:
                    return new Vector3(vector.x, vector.y, vector.z);
                default:
                    return new Vector3(vector.x, vector.y, vector.z);
            }
        }

        #endregion

        #region Others

        public static Vector2 Position2D(this Transform transform, Axis axis = Axis.Z) {
            return transform.position.ToV2(axis);
        }

        public static Vector3 Position2D(this Transform transform, Vector2 position, Axis axis = Axis.Z) {
            Vector3 output = transform.position;
            switch (axis) {
                case Axis.X:
                    output = transform.position.Override(position, Axis.Y, Axis.Z);
                    break;
                case Axis.Y:
                    output = transform.position.Override(position, Axis.X, Axis.Z);
                    break;
                case Axis.Z:
                    output = transform.position.Override(position, Axis.X, Axis.Y);
                    break;
                default:
                    break;
            }
            return output;
        }

        public static int Find(this int[] array, int value) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i] == value) {
                    return i;
                }
            }
            return -1;
        }

        public static float Positive(this float number) {
            if (number < 0) {
                number = 0;
            }
            return number;
        }

        public static T[] ToArray<T>(this Nullable<T>[] array) where T : struct {
            T[] returnBack = new T[array.Length];
            for (int i = 0; i < array.Length; i++) {
                returnBack[i] = array[i].Value;
            }
            return returnBack;
        }

        public static Nullable<T>[] ToNullableArray<T>(this T[] array) where T : struct {
            Nullable<T>[] returnBack = new Nullable<T>[array.Length];
            for (int i = 0; i < array.Length; i++) {
                returnBack[i] = (Nullable<T>)array[i];
            }
            return returnBack;
        }

        public static List<T> ToList<T>(this List<Nullable<T>> list) where T : struct {
            List<T> returnBack = new List<T>();
            for (int i = 0; i < list.Count; i++) {
                returnBack.Add(list[i].Value);
            }
            return returnBack;
        }

        public static T2[] Individually<T1, T2>(this T1[] array, BasicDelegateReturn<T1, T2> function) { // Apply a function (with 1 argument) to an Array
            T2[] result = new T2[array.Length];
            for (int i = 0; i < array.Length; i++) {
                result[i] = function(array[i]);
            }
            return result;
        }

        public static Vector3 Redirect(this Vector3 vector, Vector3 direction) {
            float angle = Vector3.SignedAngle(vector, direction, Vector3.up);
            return Quaternion.AngleAxis(angle, Vector3.up) * vector;
        }

        public static Vector3 Redirect(this Vector3 vector, Vector3 firstDirection, Vector3 lastDirection) {
            float angle = Vector3.SignedAngle(firstDirection, lastDirection, Vector3.up);
            return Quaternion.AngleAxis(angle, Vector3.up) * vector;
        }

        public static bool Contains(this LayerMask layerMask, int layer) {
            return layerMask == (layerMask | (1 << layer));
        }

        public static bool IsInside(this float number, float min, float max) {
            if (number > min && number < max) {
                return true;
            }
            return false;
        }

        public static T Random<T>(this T[] array) {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static bool Contains<T>(this T[] array, T value) {
            for (int i = 0; i < array.Length; i++) {
                if (array[i].Equals(value)) {
                    return true;
                }
            }
            return false;
        }

        public static bool Compare(this BasicComparison comparison, float number1, float number2) {
            switch (comparison) {
                case BasicComparison.LESS:
                    if (number1 < number2) { return true; }
                    break;
                case BasicComparison.GREATER:
                    if (number1 > number2) { return true; }
                    break;
                default:
                    break;
            }
            return false;
        }

        public static bool Compare(this Comparison comparison, float number, float source) {
            switch (comparison) {
                case Comparison.EQUAL:
                    if (number == source) { return true; }
                    break;
                case Comparison.DIFFERENT:
                    if (number != source) { return true; }
                    break;
                case Comparison.LESS:
                    if (number < source) { return true; }
                    break;
                case Comparison.LESS_EQUAL:
                    if (number <= source) { return true; }
                    break;
                case Comparison.GREATER:
                    if (number > source) { return true; }
                    break;
                case Comparison.GREATER_EQUAL:
                    if (number >= source) { return true; }
                    break;
                default:
                    break;
            }
            return false;
        }

        public static Color Override(this Color color, float value, Axis axis = Axis.W) {
            switch (axis) {
                case Axis.X:
                    return new Color(value, color.g, color.b, color.a);
                case Axis.Y:
                    return new Color(color.r, value, color.b, color.a);
                case Axis.Z:
                    return new Color(color.r, color.g, value, color.a);
                case Axis.W:
                    return new Color(color.r, color.g, color.b, value);
            }
            return color;
        }

        public static void SwapKey<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey) {
            if (!dictionary.ContainsKey(oldKey)) { Debug.LogWarning("Key not found : " + oldKey); return; }

            TValue value = dictionary[oldKey];
            if (dictionary.ContainsKey(newKey)) { dictionary[oldKey] = dictionary[newKey]; }
            else { dictionary.Remove(oldKey); }
            dictionary[newKey] = value;
        }

        #endregion

        #endregion

        #region Utilities

        public static float Remap(this float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static float InverseLerpUnclamped(float a, float b, float t) {
            //if (b - a == 0) { throw new DivideByZeroException(); }
            if (b - a == 0) { return 1f; }
            float value = (t - a) / (b - a);
            return value;
        }

        public static float AcuteAngle(float angle) {
            if (angle > 180f) {
                angle = angle - 360f;
            }

            return angle;
        }

        public static Vector3 AcuteAngle(Vector3 angle) {
            return new Vector3(AcuteAngle(angle.x), AcuteAngle(angle.y), AcuteAngle(angle.z));
        }

        public static float PositiveAngle(float angle) {
            while (angle < 0) {
                angle = 360f + angle;
            }
            return angle;
        }

        public static int Ponder(params float[] weight) {
            float totWeight = 0;

            for (int i = 0; i < weight.Length; i++) {
                totWeight += weight[i];
            }

            if (totWeight < 1f) {
                totWeight = 1f;
            }

            float random = UnityEngine.Random.Range(0, totWeight);

            for (int i = 0; i < weight.Length; i++) {
                if (random < weight[i]) {
                    return i;
                }
                random -= weight[i];
            }

            return -1;
        }

        /// <summary>
        /// Return a random number from argument
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static float RandomFloat(params float[] numbers) {
            int rand = UnityEngine.Random.Range(0, numbers.Length);
            return numbers[rand];
        }

        public static void SerializeInterface<T1, T2>(ref T2 input, ref T1 dummy) where T1 : class where T2 : class {
            if (dummy is T2) {
                input = dummy as T2;
            } else {
                dummy = null;
            }
        }

        public static Transform FindElderlyByTag(this Transform target) {
            string tag = target.tag;
            if (tag == "Untagged") { return target; }
            while (target.parent != null && target.parent.CompareTag(tag)) {
                target = target.parent;
            }
            return target;
        }

        public static bool IsValid(this IValid obj) {
            return obj?.IsValid ?? false;
        }

        public static Vector2 DirToV2(Direction direction) {
            switch (direction) {
                case Direction.UP:
                    return Vector2.up;
                case Direction.RIGHT:
                    return Vector2.right;
                case Direction.DOWN:
                    return Vector2.down;
                case Direction.LEFT:
                    return Vector2.left;
                default:
                    return Vector2.zero;
            }
        }

        public static Vector2Int DirToV2I(Direction direction) {
            return DirToV2(direction).ToV2I();
        }

        public static Direction Inverse(this Direction direction) {
            return (Direction)(((int)direction + 2) % 4);
        }

        #endregion

        #region Print

        public static void Print(DebugType type, char separator, params object[] strings) {
            Action<object> debug;
            switch (type) {
                case DebugType.WARNING:
                    debug = Debug.LogWarning;
                    break;
                case DebugType.ERROR:
                    debug = Debug.LogError;
                    break;
                default:
                    debug = Debug.Log;
                    break;
            }
            string output = "";
            for (int i = 0; i < strings.Length - 1; i++) {
                output += strings[i].ToString();
                output += " " + separator + " ";
            }
            output += strings[^1].ToString();
            if (output == "") { return; }
            debug(output);
        }

        public static void Print(DebugType type = DebugType.NORMAL, params object[] strings) {
            Print(type, '.', strings);
        }

        public static void Print(params object[] strings) {
            Print(DebugType.NORMAL, '.', strings);
        }

        public static void Print(char separator, params object[] strings) {
            Print(DebugType.NORMAL, separator, strings);
        }

        public static void Hurl<T>(this T hurler, DebugType type = DebugType.NORMAL) where T : UnityEngine.Object {
            string hurl = _hurlable.Random();
            hurler.Hurl(hurl, type);
        }

        public static void Hurl<T>(this T hurler, string message, DebugType type = DebugType.NORMAL) where T : UnityEngine.Object {
            Print(type, "<b>" + hurler.name + "</b> hurled at you : <b>" + message + "</b>");
        }

        public static string Print<T>(this List<T> list) {
            string output = "";
            for (int i = 0; i < list.Count; i++) {
                output += "[" + list[i].ToString() + "]";
            }
            return output;
        }

        public static string Print<T>(this T[] array) {
            string output = "";
            for (int i = 0; i < array.Length; i++) {
                output += "[" + array[i].ToString() + "]";
            }
            return output;
        }

        #endregion

        #region Coroutines

        public static IEnumerator Delay<T1, T2, T3>(BasicDelegate<T1, T2, T3> function, T1 arg1, T2 arg2, T3 arg3, float time) {
            if (time <= 0f) { function(arg1, arg2, arg3); yield break; }
            yield return new WaitForSeconds(time);
            function(arg1, arg2, arg3);
        }

        public static IEnumerator Delay<T1, T2>(BasicDelegate<T1, T2> function, T1 arg1, T2 arg2, float time) {
            if (time <= 0f) { function(arg1, arg2); yield break; }
            yield return new WaitForSeconds(time);
            function(arg1, arg2);
        }

        public static IEnumerator Delay<T>(BasicDelegate<T> function, T arg, float time) {
            if (time <= 0f) { function(arg); yield break; }
            yield return new WaitForSeconds(time);
            function(arg);
        }

        public static IEnumerator Delay(BasicDelegate function, float time) {
            if (time <= 0f) { function(); yield break; }
            yield return new WaitForSeconds(time);
            function();
        }

        public static IEnumerator UnscaledDelay<T>(BasicDelegate<T> function, T arg, float time) {
            if (time <= 0f) { function(arg); yield break; }
            yield return new WaitForSecondsRealtime(time);
            function(arg);
        }

        public static IEnumerator UnscaledDelay(BasicDelegate function, float time) {
            if (time <= 0f) { function(); yield break; }
            yield return new WaitForSecondsRealtime(time);
            function();
        }

        public static IEnumerator Delay<T1, T2>(BasicDelegate<T1, T2> function, T1 arg1, T2 arg2, Coroutine routine) {
            yield return routine;
            function(arg1, arg2);
        }

        public static IEnumerator Delay<T>(BasicDelegate<T> function, T arg, Coroutine routine) {
            yield return routine;
            function(arg);
        }

        public static IEnumerator Delay(BasicDelegate function, Coroutine routine) {
            yield return routine;
            function();
        }

        public static IEnumerator DelayOneFrame(BasicDelegate function) {
            yield return null;
            function();
        }
        #endregion

        #region Components

        /// <summary>
        /// Get a component in target or in the ColliderRoot Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetComponentInRoot<T>(this GameObject obj, out T component) where T : class {
            component = obj.GetComponent<T>();
            if (component != null) {
                return component;
            }
            component = obj.GetComponent<RootProxy>()?.Root.GetComponent<T>() ?? null;
            if (component != null) {
                return component;
            }
            return null;
        }

        /// <summary>
        /// Get a component in target or in the ColliderRoot Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetComponentInRoot<T>(this GameObject obj) where T : class {
            T component = obj.GetComponent<T>();
            if (component != null) {
                return component;
            }
            component = obj.GetComponent<RootProxy>()?.Root.GetComponent<T>() ?? null;
            if (component != null) {
                return component;
            }
            return null;
        }

        /// <summary>
        /// Get a component in target or in the ColliderRoot Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetComponentInRoot<T>(this GameObject obj, out GameObject root) where T : class {
            T component = obj.GetComponent<T>();
            if (component != null) {
                root = obj;
                return component;
            }
            root = obj.GetComponent<RootProxy>()?.Root;
            component = root?.GetComponent<T>() ?? null;
            if (component != null) {
                return component;
            }
            return null;
        }

        public static bool TryGetRootComponent<T>(this GameObject obj, out T component) {
            if (obj.TryGetComponent(out component)) {
                return true;
            }
            if (obj.GetRoot().TryGetComponent(out component)) {
                return true;
            }
            return false;
        }

        public static GameObject GetRoot(this GameObject obj) {
            return obj.GetComponent<RootProxy>()?.Root ?? obj;
        }

        #endregion
    }

    public class ReadOnlyAttribute : PropertyAttribute {

    }
}