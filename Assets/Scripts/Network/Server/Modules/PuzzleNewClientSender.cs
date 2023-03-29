using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class PuzzleNewClientSender : MonoBehaviour {
        [SerializeField] PuzzleInformation _serverPuzzle;

        void Start() {
            ServerPlayersList.OnNewClient += _OnNewClient;
        }

        private void OnDestroy() {
            ServerPlayersList.OnNewClient -= _OnNewClient;
        }

        private void _OnNewClient(Client client) {
            {
                S_PuzzlePacket packet = new S_PuzzlePacket();
                packet.size = _serverPuzzle.Size;
                packet.piecesScale = _serverPuzzle.PieceScale;

                ServerCore.Send(client.peer, packet);
            }

            {
                S_PuzzleImagePacket packet = new S_PuzzleImagePacket();
                packet.texture = _serverPuzzle.Texture;

                ServerCore.Send(client.peer, packet);
            }
        }
    }
}
