using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour {
    [Header("System")]
    [SerializeField] Piece _piece;
    [SerializeField] DraggableParent _assembledParent;
    [SerializeField] UnityEvent _onFinish;

    [Header("Puzzle")]
    [SerializeField] Texture _texture;
    [SerializeField] Vector2Int _puzzleSize = Vector2Int.one;
    [SerializeField] Vector2 _puzzleScale = Vector2.one;

    List<Piece> _pieces = new List<Piece>();
    Vector2 _pieceSize = Vector2.zero;

    public Vector2 PieceSize => _pieceSize;
    public DraggableParent AssembledParent => _assembledParent;
    public bool Finished => transform.childCount <= 1;
    public event UnityAction OnFinish { add => _onFinish.AddListener(value); remove => _onFinish.RemoveListener(value); }

    void Start() {
        _pieceSize = _puzzleScale / _puzzleSize;

        GeneratePuzzle(transform.position);
    }

    public void GeneratePuzzle(Vector3 origin) {
        //origin -= new Vector3((_puzzleSize.x * _pieceSize.x) / 2f, (_puzzleSize.y * _pieceSize.y) / 2f, 0f);
        for (int y = 0; y < _puzzleSize.y; ++y) {
            for (int x = 0; x < _puzzleSize.x; ++x) {
                Vector3 position = new Vector3(Random.Range(-_puzzleSize.x * _pieceSize.x / 2f, _puzzleSize.x * _pieceSize.x / 2f) + origin.x, Random.Range(-_puzzleSize.y * _pieceSize.y / 2f, _puzzleSize.y * _pieceSize.y / 2f) + origin.y, origin.z);
                Piece piece = Instantiate(_piece.gameObject, position, Quaternion.identity, transform)
                    .GetComponent<Piece>();
                piece.transform.localScale = new Vector3(_pieceSize.x, _pieceSize.y, piece.transform.localScale.z);
                piece.PuzzlePosition = new Vector2Int(x, y);
                piece.Puzzle = this;
                piece.OnAssemble += Verify;
                piece.gameObject.name = "Piece #" + x + ":" + y;
                _pieces.Add(piece);

                Material material = piece.GetComponent<Renderer>().material;
                material.SetTexture("_Texture", _texture);
                material.SetVector("_UVPosition", new Vector4((x + 1) / (float)_puzzleSize.x, (y + 1) / (float)_puzzleSize.y));
                material.SetVector("_UVSize", new Vector4(-1f / _puzzleSize.x, -1f / _puzzleSize.y));
            }
        }
    }

    private void Verify() {
        if (Finished) {
            _onFinish?.Invoke();
        }
    }
}
