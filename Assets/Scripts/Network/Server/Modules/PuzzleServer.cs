using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

namespace ServerModules {
    public class PuzzleServer : MonoBehaviour {
        [SerializeField] Puzzle _puzzle;
        [SerializeField] float _netTimer = 1f / 5f;

        float _timer = 0f;

        private void Start() {
            _puzzle.GeneratePuzzle(_puzzle.transform.position);
            SendWholePuzzle(_puzzle);
        }

        private void Update() {
            _timer -= Time.deltaTime;

            while (_timer <= 0f) {
                //SendPiecePhysic(_puzzle);
                ServerCore.SendAll(BuildPiecesPacket(_puzzle), ServerCore.ClientPeer);

                _timer += _netTimer;
            }
        }

        public void SendWholePuzzle(Puzzle puzzle) {
            S_WholePuzzlePacket packet = new S_WholePuzzlePacket();
            Dictionary<Vector2Int, Piece> pieces = puzzle.Pieces;

            packet.pieces = new List<S_WholePuzzlePacket.Piece>();
            foreach (var element in pieces) {
                S_WholePuzzlePacket.Piece piece = new S_WholePuzzlePacket.Piece();
                piece.position = element.Key;
                piece.gates = element.Value.Gates;

                packet.pieces.Add(piece);
            }

            ServerCore.SendAll(packet, new ENet.Peer());
        }

        public S_PiecesPacket BuildPiecesPacket(Puzzle puzzle) {
            S_PiecesPacket packet = new S_PiecesPacket();
            Dictionary<Vector2Int, Piece> pieces = puzzle.Pieces;

            packet.pieces = new List<(Vector2Int, PhysicState)>();
            foreach (var piece in pieces) {
                PhysicState state = GetPhysicState(piece.Value.Rigidbody);
                packet.pieces.Add(new (piece.Key, state));
            }

            return packet;
        }

        public PhysicState GetPhysicState(Rigidbody rb) {
            PhysicState physic = new PhysicState();
            physic.position = rb.position;
            physic.velocity = rb.velocity;
            physic.rotation = rb.rotation;
            physic.angularVelocity = rb.angularVelocity;

            return physic;
        }
    }
}
