using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class StartGameReceiver : MonoBehaviour {
        List<uint> _isReady = new List<uint>();

        void Start() {
            ServerCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ServerCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
            if (opcode == Protocols.Opcode.C_START_GAME) {
                if (!ServerCore.IsHost(peer)) { return; }

                S_IsReadyPacket packet = new S_IsReadyPacket();

                ServerCore.SendAll(packet);

            } else if (opcode == Protocols.Opcode.C_READY) {
                uint id = ServerPlayersList.FindClient(peer).Value.id;
                if (!_isReady.Contains(id)) {
                    _isReady.Add(id);
                }

                if (_isReady.Count != ServerPlayersList.Count) { return; }

                S_StartGamePacket packet = new S_StartGamePacket();

                ServerCore.SendAll(packet);
            }
        }
    }
}
