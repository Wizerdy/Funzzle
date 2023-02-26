using System;
using System.Collections;
using System.Collections.Generic;
using ENet;
using ToolsBoxEngine.BetterEvents;
using UnityEngine;
using UnityEngine.Events;

public class ServerCore : MonoBehaviour {
    [SerializeField] int _maxPlayers = 4;

    static ServerCore _instance;

    static ENet.Host _server = new ENet.Host();
    static List<Peer> _peers = new List<Peer>();

    static BetterEvent _onCreate = new BetterEvent();
    static BetterEvent<ENet.Peer> _onConnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<ENet.Peer> _onDisconnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<ENet.Peer, Protocols.Opcode, List<byte>> _onReceive = new BetterEvent<ENet.Peer, Protocols.Opcode, List<byte>>();

    public static bool ServerRunning => _server.IsSet;
    public static event UnityAction OnCreate { add => _onCreate += value; remove => _onCreate += value; }
    public static event UnityAction<ENet.Peer> OnConnect { add => _onConnect += value; remove => _onConnect += value; }
    public static event UnityAction<ENet.Peer> OnDisconnect { add => _onDisconnect += value; remove => _onDisconnect += value; }
    public static event UnityAction<ENet.Peer, Protocols.Opcode, List<byte>> OnReceive { add => _onReceive += value; remove => _onReceive += value; }

    #region Unity Callbacks

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    void Start() {
        if (!ENet.Library.Initialize())
            throw new Exception("Failed to initialize ENet");
    }

    private void OnDestroy() {
        if (!_server.IsSet) { return; }
        _server.Flush();
        _server.Dispose();
    }

    void FixedUpdate() {
        if (!_server.IsSet) { return; }

        ENet.Event evt;

        if (_server.Service(0, out evt) > 0) {
            do {
                Peer peer = evt.Peer;
                switch (evt.Type) {
                    case ENet.EventType.None:
                        break;

                    case ENet.EventType.Connect:
                        _peers.Add(peer);
                        Debug.Log("Client connected - ID: " + peer.ID + ", IP: " + peer.IP);

                        _onConnect.Invoke(peer);
                        break;

                    case ENet.EventType.Disconnect:
                        _peers.Remove(peer);
                        Debug.Log("Client disconnected - ID: " + peer.ID + ", IP: " + peer.IP);

                        _onDisconnect.Invoke(peer);
                        break;

                    case ENet.EventType.Timeout:
                        _peers.Remove(peer);
                        Debug.Log("Client timeout - ID: " + peer.ID + ", IP: " + peer.IP);

                        _onDisconnect.Invoke(peer);
                        break;

                    case ENet.EventType.Receive:
                        Debug.Log("Packet received from - ID: " + peer.ID + ", IP: " + peer.IP + ", Channel ID: " + evt.ChannelID + ", Data length: " + evt.Packet.Length);

                        byte[] array = new byte[1024];
                        evt.Packet.CopyTo(array);
                        List<byte> packet = new List<byte>();
                        packet.AddRange(array);
                        int offset = 0;
                        Protocols.Opcode opcode = (Protocols.Opcode)packet.Unserialize_u8(ref offset);

                        _onReceive.Invoke(peer, opcode, packet);

                        evt.Packet.Dispose();
                        break;
                }
            } while (_server.CheckEvents(out evt) > 0);
        }
    }

    #endregion

    public static bool CreateServer(ushort port) {
        Address address = new Address();

        address.Port = port;
        _server.Create(address, _instance._maxPlayers);
        Debug.Log("Server created at port : " + address.Port);
        _onCreate.Invoke();
        return true;
    }

    public static void Send(Peer peer, Protocols.IPacket packet) {
        if (!peer.IsSet) { return; }
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        peer.Send(0, ref epacket);
    }

    public static void Send(Protocols.IPacket packet) {
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        for (int i = 0; i < _peers.Count; i++) {
            if (!_peers[i].IsSet) { continue; }
            _peers[i].Send(0, ref epacket);
        }
    }
}
