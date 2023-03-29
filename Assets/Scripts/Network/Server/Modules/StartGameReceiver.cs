using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class StartGameReceiver : MonoBehaviour {
        void Start() {
            ServerCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ServerCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
            if (opcode != Protocols.Opcode.C_START_GAME) { return; }
            if (!ServerCore.IsHost(peer)) { return; }

            S_StartGamePacket packet = new S_StartGamePacket();

            ServerCore.SendAll(packet);
        }
    }
}