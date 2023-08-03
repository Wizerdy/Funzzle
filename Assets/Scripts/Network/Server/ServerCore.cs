using System;
using System.Collections;
using System.Collections.Generic;
using ENet;
using ToolsBoxEngine;
using ToolsBoxEngine.BetterEvents;
using UnityEngine;
using UnityEngine.Events;

public class ServerCore : MonoBehaviour {
    [SerializeField] PlayerInformation _playerInformation;
    [SerializeField] int _maxPlayers = 4;

    static ServerCore _instance;

    ENet.Host _server = new ENet.Host();
    List<Peer> _peers = new List<Peer>();

    static BetterEvent _onCreate = new BetterEvent();
    static BetterEvent<ENet.Peer> _onConnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<ENet.Peer> _onDisconnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<ENet.Peer, Protocols.Opcode, List<byte>> _onReceive = new BetterEvent<ENet.Peer, Protocols.Opcode, List<byte>>();
    static BetterEvent<ENet.Peer, Protocols.Opcode> _onSend = new BetterEvent<ENet.Peer, Protocols.Opcode>();

    public static bool ServerRunning => _instance._server.IsSet;
    public static Peer ClientPeer => new Peer();

    public static event UnityAction OnCreate { add => _onCreate += value; remove => _onCreate += value; }
    public static event UnityAction<ENet.Peer> OnConnect { add => _onConnect += value; remove => _onConnect += value; }
    public static event UnityAction<ENet.Peer> OnDisconnect { add => _onDisconnect += value; remove => _onDisconnect += value; }
    public static event UnityAction<ENet.Peer, Protocols.Opcode, List<byte>> OnReceive { add => _onReceive += value; remove => _onReceive += value; }
    public static event UnityAction<ENet.Peer, Protocols.Opcode> OnSend { add => _onSend += value; remove => _onSend += value; }

    #region Unity Callbacks

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
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

        // Sham Server
        {
            ENet.Packet packet;
            while (ShamClientCore.CheckSendQueue(out packet) > 0) {
                Receive(new Peer(), packet);
            }
        }

        // Network Server
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
                        //Debug.Log("Packet received from - ID: " + peer.ID + ", IP: " + peer.IP + ", Channel ID: " + evt.ChannelID + ", Data length: " + evt.Packet.Length);
                        Receive(peer, evt.Packet);

                        evt.Packet.Dispose();
                        break;
                }
            } while (_server.CheckEvents(out evt) > 0);
        }
    }

    private void Receive(ENet.Peer peer, ENet.Packet epacket) {
        byte[] array = new byte[epacket.Length];
        epacket.CopyTo(array);
        List<byte> packet = new List<byte>();
        packet.AddRange(array);
        int offset = 0;
        Protocols.Opcode opcode = (Protocols.Opcode)packet.Unserialize_u8(ref offset);

        Debug.Log("(S) Packet Received : " + opcode.ToString() + " (" + epacket.Length + ")");

        _onReceive.Invoke(peer, opcode, packet);
    }

    #endregion

    public static bool CreateServer(ushort port) {
        Address address = new Address();

        address.Port = port;
        _instance._server.Create(address, _instance._maxPlayers);
        Debug.Log("Server created at " + address.GetHost() + " : " + address.Port);
        _onCreate.Invoke();
        return true;
    }

    public static void Send(Peer peer, ENet.Packet packet) {
        if (IsHost(peer)) { ShamClientCore.Receive(packet); return; }
        peer.Send(0, ref packet);
    }

    public static void Send(Peer peer, Protocols.IPacket packet) {
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        Debug.Log("(S) Packet Send to " + (peer.IsSet ? peer.IP : "localhost") + " : " + packet.Opcode + " (" + epacket.Length + ")");
        _onSend.Invoke(peer, packet.Opcode);
        Send(peer, epacket);
    }

    public static void SendAll(Protocols.IPacket packet, params Peer[] peerToIgnore) {
        //ENet.Packet epacket = Protocols.BuildPacket(packet);
        for (int i = 0; i < _instance._peers.Count; i++) {
            if (peerToIgnore.Contains(_instance._peers[i])) { continue; }
            Send(_instance._peers[i], packet);
        }

        // Sham Client
        if (!peerToIgnore.Contains(new Peer())) {
            Send(new Peer(), packet);
        }
    }

    public static void SendAll(Protocols.IPacket packet) {
        SendAll(packet, new Peer[] {});
    }

    public static bool IsHost(Peer peer) {
        return !peer.IsSet;
    }
}
