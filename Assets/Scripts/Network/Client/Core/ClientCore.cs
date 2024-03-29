using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ToolsBoxEngine.BetterEvents;
using UnityEngine.Events;
using UnityEditor;
using ENet;

public class ClientCore : MonoBehaviour {
    [SerializeField] PlayerInformation _playerInformation;

    static ClientCore _instance;

    static BetterEvent<ENet.Peer> _onTimeout = new BetterEvent<ENet.Peer>();
    static BetterEvent<ENet.Peer> _onConnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<Protocols.Opcode, List<byte>> _onReceive = new BetterEvent<Protocols.Opcode, List<byte>>();
    static BetterEvent<Protocols.Opcode> _onSend = new BetterEvent<Protocols.Opcode>();

    ENet.Host m_enetHost = new ENet.Host();
    ENet.Peer peer;

    public static event UnityAction<ENet.Peer> OnTimeout { add => _onTimeout += value; remove => _onTimeout -= value; }
    public static event UnityAction<ENet.Peer> OnConnect { add => _onConnect += value; remove => _onConnect -= value; }
    public static event UnityAction<Protocols.Opcode, List<byte>> OnReceive { add => _onReceive += value; remove => _onReceive -= value; }
    public static event UnityAction<Protocols.Opcode> OnSend { add => _onSend += value; remove => _onSend -= value; }

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

    private void Start() {
        if (!ENet.Library.Initialize())
            throw new Exception("Failed to initialize ENet");
    }

    private void OnDestroy() {
        if (!m_enetHost.IsSet) { return; }
        m_enetHost.Dispose();
    }

    void FixedUpdate() {
        // Sham Client
        {
            ENet.Packet packet;
            while (ShamClientCore.CheckReceiveQueue(out packet) > 0) {
                Receive(packet);
            }
        }

        // Network Client
        if (!m_enetHost.IsSet) { return; }

        ENet.Event evt;

        if (m_enetHost.Service(0, out evt) > 0) {
            do {
                switch (evt.Type) {
                    case ENet.EventType.None:
                        Debug.Log("?");
                        break;

                    case ENet.EventType.Connect:
                        //Debug.Log("Connect");

                        _onConnect.Invoke(peer);
                        break;

                    case ENet.EventType.Disconnect:
                        Debug.Log("Disconnect");
                        break;

                    case ENet.EventType.Receive:
                        Receive(evt.Packet);

                        evt.Packet.Dispose();
                        break;

                    case ENet.EventType.Timeout:
                        _onTimeout.Invoke(evt.Peer);
                        Debug.Log("Timeout");
                        break;
                }
            }
            while (m_enetHost.CheckEvents(out evt) > 0);
        }
    }

    private void Receive(ENet.Packet epacket) {
        byte[] array = new byte[epacket.Length];
        epacket.CopyTo(array);
        List<byte> packet = new List<byte>();
        packet.AddRange(array);
        int offset = 0;
        Protocols.Opcode opcode = (Protocols.Opcode)packet.Unserialize_u8(ref offset);

        Debug.Log("(C) Packet Received : " + opcode.ToString() + " (" + epacket.Length + ")" + '\n' +
            packet.Print());
        _onReceive.Invoke(opcode, packet);
    }

    #endregion

    public static bool Connect(string addressString, ushort port) {
        if (!_instance.m_enetHost.IsSet) {
            _instance.m_enetHost.Create(1, 0);
        }

        Debug.Log("Trying to connect to " + addressString + ":" + port);
        ENet.Address address = new ENet.Address();
        if (!address.SetHost(addressString)) {
            Debug.Log("Connection failed");
            return false;
        }

        address.Port = port;

        _instance.peer = _instance.m_enetHost.Connect(address, 0);
        return true;
    }

    public static void Send(Protocols.IPacket packet) {
        if (_instance._playerInformation.IsServer) { _onSend.Invoke(packet.Opcode); ShamClientCore.Send(packet); return; }

        if (!_instance.peer.IsSet) { return; }
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        Debug.Log("(C) Packet Send : " + packet.Opcode.ToString() + " (" + epacket.Length + ")");
        _instance.peer.Send(0, ref epacket);
        _onSend.Invoke(packet.Opcode);
    }
}
