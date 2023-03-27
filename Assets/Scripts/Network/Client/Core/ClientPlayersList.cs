using ENet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ToolsBoxEngine.BetterEvents;

public struct Player {
    public uint id;
    public string name;
}

public class ClientPlayersList : MonoBehaviour {
    static Dictionary<uint, Player> _players = new Dictionary<uint, Player>();

    static BetterEvent<Player> _onNewPlayer = new BetterEvent<Player>();
    static BetterEvent<Player> _onPlayerLeft = new BetterEvent<Player>();

    public static Dictionary<uint, Player> Players => new Dictionary<uint, Player>(_players);

    public static event UnityAction<Player> OnNewPlayer { add => _onNewPlayer += value; remove => _onNewPlayer -= value; }
    public static event UnityAction<Player> OnPlayerLeft { add => _onPlayerLeft += value; remove => _onPlayerLeft -= value; }

    private void Start() {
        ClientCore.OnReceive += _OnReceive;
    }

    private void OnDestroy() {
        ClientCore.OnReceive -= _OnReceive;
    }

    private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
        switch (opcode) {
            case Protocols.Opcode.S_NEW_PLAYER:
                {
                    S_NewPlayerPacket packet = S_NewPlayerPacket.Unserialize(bytes);

                    Player player = new Player();
                    player.id = packet.id;
                    player.name = packet.name;

                    AddPlayer(player);
                }
                break;
            case Protocols.Opcode.S_PLAYER_LEFT:
                {
                    S_PlayerLeftPacket packet = S_PlayerLeftPacket.Unserialize(bytes);

                    RemovePlayer(packet.id);
                }
                break;
            case Protocols.Opcode.S_PLAYERS_LIST:
                {
                    S_PlayersListPacket packet = S_PlayersListPacket.Unserialize(bytes);

                    for (int i = 0; i < packet.players.Count; i++) {
                        Player player = new Player();
                        player.id = packet.players[i].id;
                        player.name = packet.players[i].name;

                        AddPlayer(player);
                    }
                }
                break;
        }
    }

    private void AddPlayer(Player player) {
        _players.Add(player.id, player);
        _onNewPlayer.Invoke(player);
    }

    private void RemovePlayer(uint id) {
        if (!_players.ContainsKey(id)) { return; }

        Player player = _players[id];
        _players.Remove(id);
        _onPlayerLeft.Invoke(player);
    }
}
