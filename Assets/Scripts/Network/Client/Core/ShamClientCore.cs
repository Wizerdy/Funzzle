using ENet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ToolsBoxEngine.BetterEvents;

public static class ShamClientCore {
    static Queue<ENet.Packet> _receiveQueue = new Queue<ENet.Packet>();
    static Queue<ENet.Packet> _sendQueue = new Queue<ENet.Packet>();

    public static void Receive(ENet.Packet packet) {
        _receiveQueue.Enqueue(packet);
    }

    public static void Send(Protocols.IPacket packet) {
        ENet.Packet epacket = Protocols.BuildPacket(packet);
        Debug.Log("(SC) Packet Send : " + packet.Opcode.ToString() + " (" + epacket.Length + ")");
        _sendQueue.Enqueue(epacket);
    }

    public static int CheckReceiveQueue(out ENet.Packet packet) {
        if (_receiveQueue.Count == 0) { packet = new ENet.Packet(); return 0; }
        packet = _receiveQueue.Dequeue();

        return _receiveQueue.Count + 1;
    }

    public static int CheckSendQueue(out ENet.Packet packet) {
        if (_sendQueue.Count == 0) { packet = new Packet(); return 0; }
        packet = _sendQueue.Dequeue();

        return _sendQueue.Count + 1;
    }
}
