using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNetwork : MonoBehaviour {
    [Serializable]
    public struct GUIDebug {
        public string text;
        public float time;
        internal double apparitionTime;
        public Color color;
    }

    static DebugNetwork _instance;

    [SerializeField] float _debugTime = 10f;
    [SerializeField] Color _inColor = Color.green;
    [SerializeField] Color _outColor = Color.red;

    List<GUIDebug> _debugs;

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void Start() {
        ServerCore.OnReceive += _OnServerReceive;
        ServerCore.OnSend += _OnServerSend;
        ClientCore.OnReceive += _OnClientReceive;
        ClientCore.OnSend += _OnClientSend;

        _debugs = new List<GUIDebug>();
    }

    private void OnDestroy() {
        ServerCore.OnReceive -= _OnServerReceive;
        ServerCore.OnSend -= _OnServerSend;
        ClientCore.OnReceive -= _OnClientReceive;
        ClientCore.OnSend -= _OnClientSend;
    }

    private void _OnServerReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
        CreateDebug("(S) " + (ServerPlayersList.FindClient(peer)?.name ?? "host") + " >> " + opcode, _debugTime, _inColor);
    }

    private void _OnServerSend(Peer peer, Protocols.Opcode opcode) {
        CreateDebug("(S) " + (ServerPlayersList.FindClient(peer)?.name ?? "host") + " << " + opcode, _debugTime, _outColor);
    }

    private void _OnClientReceive(Protocols.Opcode opcode, List<byte> bytes) {
        CreateDebug("(C) >> " + opcode, _debugTime, _inColor);
    }

    private void _OnClientSend(Protocols.Opcode opcode) {
        CreateDebug("(C) << " + opcode, _debugTime, _outColor);
    }

    public void CreateDebug(string text, float time, Color color) {
        GUIDebug debug = new GUIDebug {
            text = " " + text + " ",
            time = time,
            apparitionTime = Time.timeAsDouble,
            color = color
        };
        _debugs.Add(debug);
    }

    private void OnGUI() {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        //style.normal.background = GUI.skin.label.normal.background;
        style.normal.background = Texture2D.normalTexture;
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = 12;
        Vector2 xSize = style.CalcSize(new GUIContent("X"));
        Rect rect = new Rect(10f, 10f, 200f, xSize.y);
        for (int i = 0; i < _debugs.Count; i++) {
            if (_debugs[i].apparitionTime + (double)_debugs[i].time < Time.timeAsDouble) { _debugs.RemoveAt(i); --i; continue; }
            style.normal.textColor = _debugs[i].color;

            rect.width = Mathf.Min(style.CalcSize(new GUIContent(_debugs[i].text)).x, 200f);
            GUI.Label(rect, _debugs[i].text, style);
            rect.y += rect.height * 1.2f;
        }
    }
}
