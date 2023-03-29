using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientModules {
    public class PuzzleLoader : MonoBehaviour {
        [SerializeField] PuzzleInformation _clientPuzzle;
        [SerializeField] Puzzle _puzzleSystem;

        private void Awake() {
            if (_clientPuzzle == null) { return; }

            _puzzleSystem.Texture = _clientPuzzle.Texture;
            _puzzleSystem.PuzzleSize = _clientPuzzle.Size;
            _puzzleSystem.PieceSize = _clientPuzzle.PieceScale;
        }
    }
}
