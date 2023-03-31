using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientModules {
    public class PuzzleReceiver : MonoBehaviour {
        [SerializeField] Puzzle _puzzle;

        private void Start() {
            ClientCore.OnReceive += _OnReceive;
        }

        private void OnDestroy() {
            ClientCore.OnReceive -= _OnReceive;
        }

        private void _OnReceive(Protocols.Opcode opcode, List<byte> bytes) {
            switch (opcode) {
                case Protocols.Opcode.S_WHOLE_PUZZLE: {
                        S_WholePuzzlePacket packet = S_WholePuzzlePacket.Unserialize(bytes);

                        for (int i = 0; i < packet.pieces.Count; i++) {
                            _puzzle.CreatePiece(packet.pieces[i].position, packet.pieces[i].gates, Vector3.zero).Init();
                        }
                    }
                    break;
                case Protocols.Opcode.S_PIECES: {
                        S_PiecesPacket packet = S_PiecesPacket.Unserialize(bytes);

                        Dictionary<Vector2Int, Piece> pieces = _puzzle.Pieces;
                        Vector2Int puzzlePos;
                        for (int i = 0; i < packet.pieces.Count; i++) {
                            puzzlePos = packet.pieces[i].Item1;
                            if (!pieces.ContainsKey(puzzlePos)) { Debug.LogWarning("Piece Non Existant : " + packet.pieces[i].Item1); continue; }

                            pieces[puzzlePos].ApplyPhysicState(packet.pieces[i].Item2);
                        }
                    }
                    break;
            }
        }
    }
}