using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class SeTextureSender : MonoBehaviour {
        void Start() {
            ServerCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ServerCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
            if (opcode == Protocols.Opcode.C_PUZZLE_IMAGE) {
                C_PuzzleImagePacket packet = C_PuzzleImagePacket.Unserialize(bytes);

                S_PuzzleImagePacket toSend = new S_PuzzleImagePacket();
                toSend.texture = packet.texture;
                ServerCore.SendAll(toSend);
            } else if (opcode == Protocols.Opcode.C_PUZZLE) {
                C_PuzzlePacket packet = C_PuzzlePacket.Unserialize(bytes);

                S_PuzzlePacket toSend = new S_PuzzlePacket();
                toSend.size = packet.size;
                toSend.piecesScale = packet.scale;
                ServerCore.SendAll(toSend);
            }
        }
    }
}