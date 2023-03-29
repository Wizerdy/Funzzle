using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class SeTextureSender : MonoBehaviour {
        [SerializeField] PuzzleInformation _serverPuzzle;

        void Start() {
            ServerCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ServerCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Peer peer, Protocols.Opcode opcode, List<byte> bytes) {
            if (!ServerCore.IsHost(peer)) { return; }

            if (opcode == Protocols.Opcode.C_PUZZLE_IMAGE) {
                C_PuzzleImagePacket packet = C_PuzzleImagePacket.Unserialize(bytes);
                _serverPuzzle.Texture = packet.texture;

                S_PuzzleImagePacket toSend = new S_PuzzleImagePacket();
                toSend.texture = packet.texture;

                ServerCore.SendAll(toSend);
            } else if (opcode == Protocols.Opcode.C_PUZZLE) {
                C_PuzzlePacket packet = C_PuzzlePacket.Unserialize(bytes);
                _serverPuzzle.Size = packet.size;
                _serverPuzzle.PieceScale = packet.scale;

                S_PuzzlePacket toSend = new S_PuzzlePacket();
                toSend.size = packet.size;
                toSend.piecesScale = packet.scale;
                ServerCore.SendAll(toSend);
            }
        }
    }
}