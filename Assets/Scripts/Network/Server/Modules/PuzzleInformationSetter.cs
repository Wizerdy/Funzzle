using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerModules {
    public class PuzzleInformationSetter : MonoBehaviour {
        [SerializeField] PuzzleInformation _serverPuzzle;
        [SerializeField] Texture2D _defaultTexture;
        [SerializeField] Vector2Int _defaultSize = Vector2Int.one * 5;
        [SerializeField] Vector2 _defaultPieceScale = Vector2.one;

        private void Start() {
            if (_serverPuzzle == null) { Debug.LogWarning("No default Puzzle Information Set"); return; }

            _serverPuzzle.Texture = _defaultTexture;
            _serverPuzzle.Size = _defaultSize;
            _serverPuzzle.PieceScale = _defaultPieceScale;

            {
                S_PuzzleImagePacket packet = new S_PuzzleImagePacket();

                packet.texture = _defaultTexture;
                ServerCore.SendAll(packet);
            }

            {
                S_PuzzlePacket packet = new S_PuzzlePacket();

                packet.size = _defaultSize;
                packet.piecesScale = _defaultPieceScale;
                ServerCore.SendAll(packet);
            }
        }
    }
}
