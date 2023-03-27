using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ServerModules {
    public class NewClientSender : MonoBehaviour {
        void Start() {
            ServerPlayersList.OnNewClient += _OnNewClient;
        }

        private void OnDestroy() {
            ServerPlayersList.OnNewClient -= _OnNewClient;
        }

        private void _OnNewClient(Client client) {
            SendNewClientToOthers(client);
            SendPlayersListToNew(client);
        }

        private void SendNewClientToOthers(Client client) {
            S_NewPlayerPacket packet = new S_NewPlayerPacket();

            packet.id = client.id;
            packet.name = client.name;

            ServerCore.SendAll(packet, client.peer);
        }

        private void SendPlayersListToNew(Client client) {
            Dictionary<uint, Client> clients = ServerPlayersList.Clients;
            if (clients.Count <= 1) { return; }

            S_PlayersListPacket packet = new S_PlayersListPacket();
            packet.players = new List<S_PlayersListPacket.Player>();

            foreach (var item in clients) {
                if (client.id == item.Value.id) { continue; }
                S_PlayersListPacket.Player player = new S_PlayersListPacket.Player();
                player.id = item.Value.id;
                player.name = item.Value.name;
                packet.players.Add(player);
            }

            ServerCore.Send(client.peer, packet);
        }
    }
}
