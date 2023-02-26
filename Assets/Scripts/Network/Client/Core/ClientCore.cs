using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ToolsBoxEngine.BetterEvents;
using UnityEngine.Events;

public class ClientCore : MonoBehaviour {
    static ClientCore _instance;

    static BetterEvent<ENet.Peer> _onConnect = new BetterEvent<ENet.Peer>();
    static BetterEvent<Protocols.Opcode, List<byte>> _onReceive = new BetterEvent<Protocols.Opcode, List<byte>>();

    static ENet.Host m_enetHost = new ENet.Host();
    static ENet.Peer peer;

    public static event UnityAction<ENet.Peer> OnConnect { add => _onConnect += value; remove => _onConnect -= value; }
    public static event UnityAction<Protocols.Opcode, List<byte>> OnReceive { add => _onReceive += value; remove => _onReceive -= value; }

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

        Debug.Log(m_enetHost.NativeData);
        m_enetHost.Create(1, 0);
    }

    private void OnDestroy() {
        if (!m_enetHost.IsSet) { return; }
        m_enetHost.Dispose();
    }

    void FixedUpdate() {
        ENet.Event evt;
        if (m_enetHost.Service(0, out evt) > 0) {
            do {
                switch (evt.Type) {
                    case ENet.EventType.None:
                        Debug.Log("?");
                        break;

                    case ENet.EventType.Connect:
                        Debug.Log("Connect");

                        _onConnect.Invoke(peer);
                        break;

                    case ENet.EventType.Disconnect:
                        Debug.Log("Disconnect");
                        break;

                    case ENet.EventType.Receive:
                        byte[] array = new byte[1024];
                        evt.Packet.CopyTo(array);
                        List<byte> packet = new List<byte>();
                        packet.AddRange(array);
                        int offset = 0;
                        Protocols.Opcode opcode = (Protocols.Opcode)packet.Unserialize_u8(ref offset);

                        _onReceive.Invoke(opcode, packet);
                        evt.Packet.Dispose();
                        break;

                    case ENet.EventType.Timeout:
                        Debug.Log("Timeout");
                        break;
                }
            }
            while (m_enetHost.CheckEvents(out evt) > 0);
        }
    }

    #endregion

    public static bool Connect(string addressString, ushort port) {
        Debug.Log("Trying to connect to " + addressString + ":" + port);
        ENet.Address address = new ENet.Address();
        if (!address.SetHost(addressString)) {
            Debug.Log("Connection failed");
            return false;
        }

        address.Port = port;

        peer = m_enetHost.Connect(address, 0);
        return true;
    }

    public static void Send(Protocols.IPacket packet) {
        if (!peer.IsSet) { return; }
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        peer.Send(0, ref epacket);
    }
}
