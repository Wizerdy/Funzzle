using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Client {
    public uint id;
    public string name;
    public Peer peer;
}

public class ServerPlayersList : MonoBehaviour {
    SortedDictionary<uint, Client> _clients = new SortedDictionary<uint, Client>();

    private void Start() {
        ServerCore.OnReceive += _OnReceive;
        ServerCore.OnDisconnect += _OnDisconnect;
    }

    private void OnDestroy() {
        ServerCore.OnReceive -= _OnReceive;
        ServerCore.OnDisconnect -= _OnDisconnect;
    }

    private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
        int offset = 0;
        Protocols.TestOpcode(bytes, Protocols.Opcode.C_JOIN_GAME, ref offset);

        Client client = new Client();
        {
            C_JoinGamePacket packet = C_JoinGamePacket.Unserialize(bytes);

            client.peer = peer;
            client.name = packet.name;
            client.id = NextAvaibleId();
        }

        _clients.Add(client.id, client);

        {
            S_IdPacket packet = new S_IdPacket();
            packet.id = client.id;

            ServerCore.Send(client.peer, packet);
        }
    }

    private void _OnDisconnect(Peer peer) {
        DeleteClient(peer);
    }

    private uint NextAvaibleId() {
        uint nextId = 1;
        foreach (KeyValuePair<uint, Client> client in _clients) {
            if (nextId != client.Key) {
                return nextId;
            }
            ++nextId;
        }
        return nextId;
    }

    public Client GetClient(uint id) {
        if (id == 0) { return ClientServer(); }
        if (!_clients.ContainsKey(id)) { throw new ArgumentOutOfRangeException("No such client " + id); }
        return _clients[id];
    }

    public Client ClientServer() {
        Client client = new Client();
        client.name = PlayerInformation.Name;
        client.id = 0;
        return client;
    }

    private void DeleteClient(Peer peer) {
        foreach (KeyValuePair<uint, Client> client in _clients) {
            if (client.Value.peer.ID == peer.ID) {
                _clients.Remove(client.Key);
            }
        }
    }
}
