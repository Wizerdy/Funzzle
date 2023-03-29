using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine.BetterEvents;
using UnityEngine.Events;
using ToolsBoxEngine;
using static UnityEngine.UI.Image;

public class Puzzle : MonoBehaviour {
    [Header("System")]
    [SerializeField] Piece _piece;
    [SerializeField] DraggableParent _assembledParent;

    [Header("Puzzle")]
    [SerializeField] Texture2D _texture;
    [SerializeField] bool _keepRatio = true;
    [SerializeField] Vector2Int _puzzleSize = Vector2Int.one;
    [SerializeField] Vector2 _pieceSize = Vector2.one;
    [SerializeField] Vector2 _spawnRect = Vector2.one;
    [Space]
    [SerializeField] BetterEvent _onPieceAssemble = new BetterEvent();
    [SerializeField] BetterEvent _onFinish = new BetterEvent();

    Dictionary<Vector2Int, Piece> _pieces = new Dictionary<Vector2Int, Piece>();

    public Texture2D Texture { get => _texture; set => _texture = value; }
    public Vector2Int PuzzleSize { get => _puzzleSize; set => _puzzleSize = value; }
    public Vector2 PieceSize { get => _pieceSize; set => _pieceSize = value; }
    public DraggableParent AssembledParent => _assembledParent;
    public Dictionary<Vector2Int, Piece> Pieces => _pieces;

    public event UnityAction OnPieceAssemble { add => _onPieceAssemble += value; remove => _onPieceAssemble -= value; }
    public event UnityAction OnFinish { add => _onFinish += value; remove => _onFinish -= value; }

    void Start() {
        //GeneratePuzzle(transform.position);
    }

    public void GeneratePuzzle(Vector3 origin) {
        if (_keepRatio) {
            float ratio = (float)_texture.height / (float)_texture.width;
            _puzzleSize.y = Mathf.RoundToInt(ratio * _puzzleSize.x);
        }

        int[] gates;
        for (int y = 0; y < _puzzleSize.y; ++y) {
            for (int x = 0; x < _puzzleSize.x; ++x) {
                Vector2Int piecePosition = new Vector2Int(x, y);
                gates = GenerateGates(piecePosition);

                Vector3 position = new Vector3(Random.Range(-_spawnRect.x / 2f, _spawnRect.x / 2f) + origin.x, Random.Range(-_spawnRect.y / 2f, _spawnRect.y / 2f) + origin.y, origin.z);
                CreatePiece(piecePosition, gates, position);
            }
        }

        foreach (var item in _pieces) {
            item.Value.Init();
        }
    }

    public Piece CreatePiece(Vector2Int puzposition, int[] gates, Vector3 position) {
        if (_pieces.ContainsKey(puzposition)) { Debug.LogError("Piece already generated : " + puzposition); return null; }

        Piece piece = Instantiate(_piece.gameObject, position, Quaternion.identity, transform)
            .GetComponent<Piece>();
        piece.transform.localScale = new Vector3(_pieceSize.x, _pieceSize.y, piece.transform.localScale.z);
        piece.transform.rotation = Quaternion.LookRotation(-Vector3.forward, -Vector3.up);
        piece.PuzzlePosition = puzposition;
        piece.Puzzle = this;
        piece.Gates = gates;
        piece.Texture = _texture;
        piece.OnAssemble += Verify;
        piece.OnAssemble += _InvokeOnPieceAssemble;
        piece.gameObject.name = "Piece #" + position.x + ":" + position.y;

        _pieces.Add(puzposition, piece);
        return piece;
    }

    private int[] GenerateGates(Vector2Int position) {
        int[] gates = { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++) {
            Direction direction = (Direction)i;
            Vector2Int neighbour = position + Tools.DirToV2I(direction).UpIsDown();
            if (_pieces.ContainsKey(neighbour)) {
                gates[i] = _pieces[neighbour].Gates[(int)direction.Inverse()] * -1;
            } else {
                gates[i] = Random.Range(0, 2) == 0 ? -1 : 1;
            }
        }

        if (position.x == 0) {
            gates[(int)Direction.LEFT] = 0;
        }
        if (position.y == 0) {
            gates[(int)Direction.UP] = 0;
        }
        if (position.x == _puzzleSize.x - 1) {
            gates[(int)Direction.RIGHT] = 0;
        }
        if (position.y == _puzzleSize.y - 1) {
            gates[(int)Direction.DOWN] = 0;
        }

        return gates;
    }

    private void _InvokeOnPieceAssemble() {
        _onPieceAssemble.Invoke();
    }

    private void Verify() {
        if (CheckEnd()) {
            Debug.Log("Fin");
            _onFinish.Invoke();
        }
    }

    public bool CheckEnd() {
        foreach (KeyValuePair<Vector2Int, Piece> piece in _pieces) {
            if (!piece.Value.InPlace()) {
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _spawnRect);
    }
}
