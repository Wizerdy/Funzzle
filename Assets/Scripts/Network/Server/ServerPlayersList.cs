using ENet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ToolsBoxEngine.BetterEvents;

public struct Client {
    public uint id;
    public string name;
    public Peer peer;
}

public class ServerPlayersList : MonoBehaviour {
    static ServerPlayersList _instance;

    [SerializeField] PlayerInformation _playerInformation;

    SortedDictionary<uint, Client> _clients = new SortedDictionary<uint, Client>();

    static BetterEvent<Client> _onNewClient = new BetterEvent<Client>();

    public static int Count => _instance._clients.Count + 1;
    public static ServerPlayersList Instance => _instance;
    public static Dictionary<uint, Client> Clients => GetClients();
    public static event UnityAction<Client> OnNewClient { add => _onNewClient += value; remove => _onNewClient -= value; }

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start() {
        ServerCore.OnReceive += _OnReceive;
        ServerCore.OnDisconnect += _OnDisconnect;
    }

    private void OnDestroy() {
        ServerCore.OnReceive -= _OnReceive;
        ServerCore.OnDisconnect -= _OnDisconnect;
    }

    private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
        if (opcode == Protocols.Opcode.C_JOIN_GAME) {
            Client client = new Client();
            {
                C_JoinGamePacket packet = C_JoinGamePacket.Unserialize(bytes);

                client.peer = peer;
                client.name = packet.name;
                client.id = NextAvaibleId();
            }

            NewClient(peer, client);
        }
    }

    private void _OnDisconnect(Peer peer) {
        Client? client = FindClient(peer);
        if (client == null) { return; }
        Debug.Log("<< Client left #" + client.Value.id + " " + client.Value.name);
        DeleteClient(peer);
    }

    private void NewClient(Peer peer, Client client) {
        _clients.Add(client.id, client);
        Debug.Log(">> New client #" + client.id + " " + client.name);

        {
            S_IdPacket packet = new S_IdPacket();
            packet.id = client.id;

            ServerCore.Send(client.peer, packet);
        }

        _onNewClient.Invoke(client);
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

    public static Client GetClient(uint id) {
        if (id == 0) { return ClientServer(); }
        if (!_instance._clients.ContainsKey(id)) { throw new System.ArgumentOutOfRangeException("No such client " + id); }
        return _instance._clients[id];
    }

    public static Client ClientServer() {
        Client client = new Client();
        client.name = _instance._playerInformation.Name;
        client.id = 0;
        return client;
    }

    public static Client? FindClient(Peer peer) {
        foreach (KeyValuePair<uint, Client> client in _instance._clients) {
            if (client.Value.peer.ID == peer.ID) {
                return client.Value;
            }
        }
        return null;
    }

    private static void DeleteClient(Peer peer) {
        Client? client = FindClient(peer);
        if (client == null) { return; }
        _instance._clients.Remove(client.Value.id);
    }

    private static Dictionary<uint, Client> GetClients() {
        Dictionary<uint, Client> clients = new Dictionary<uint, Client>(_instance._clients) {
            { 0, ClientServer() }
        };
        return clients;
    }
}
