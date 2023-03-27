using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine.BetterEvents;
using UnityEngine.Events;
using ToolsBoxEngine;

public class Puzzle : MonoBehaviour {
    [Header("System")]
    [SerializeField] Piece _piece;
    [SerializeField] DraggableParent _assembledParent;
    [SerializeField] BetterEvent _onFinish = new BetterEvent();

    [Header("Puzzle")]
    [SerializeField] Texture2D _texture;
    [SerializeField] bool _keepRatio = true;
    [SerializeField] Vector2Int _puzzleSize = Vector2Int.one;
    [SerializeField] Vector2 _pieceSize = Vector2.one;
    [SerializeField] Vector2 _spawnRect = Vector2.one;
    [Space]
    [SerializeField] BetterEvent _onPieceAssemble = new BetterEvent();

    Dictionary<Vector2Int, Piece> _pieces = new Dictionary<Vector2Int, Piece>();

    public Vector2 PuzzleSize => _puzzleSize;
    public Vector2 PieceSize => _pieceSize;
    public DraggableParent AssembledParent => _assembledParent;

    public event UnityAction OnPieceAssemble { add => _onPieceAssemble += value; remove => _onPieceAssemble -= value; }
    public event UnityAction OnFinish { add => _onFinish += value; remove => _onFinish -= value; }

    void Start() {
        Debug.Log(_texture.GetRawTextureData().Length + " .. " + _texture.width + " .. " + _texture.height);
        Debug.Log(_texture.GetRawTextureData().Print());
        if (_keepRatio) {
            float ratio = (float)_texture.height / (float)_texture.width;
            _puzzleSize.y = Mathf.RoundToInt(ratio * _puzzleSize.x);
        }
        GeneratePuzzle(transform.position);
    }

    public void GeneratePuzzle(Vector3 origin) {
        //origin -= new Vector3((_puzzleSize.x * _pieceSize.x) / 2f, (_puzzleSize.y * _pieceSize.y) / 2f, 0f);
        int[] gates = { 0, 0, 0, 0 };
        for (int y = 0; y < _puzzleSize.y; ++y) {
            for (int x = 0; x < _puzzleSize.x; ++x) {
                Vector2Int piecePosition = new Vector2Int(x, y);
                gates = GeneratePiece(piecePosition);

                //Vector3 position = new Vector3(Random.Range(-_puzzleSize.x * _pieceSize.x / 2f, _puzzleSize.x * _pieceSize.x / 2f) + origin.x, Random.Range(-_puzzleSize.y * _pieceSize.y / 2f, _puzzleSize.y * _pieceSize.y / 2f) + origin.y, origin.z);
                Vector3 position = new Vector3(Random.Range(-_spawnRect.x / 2f, _spawnRect.x / 2f) + origin.x, Random.Range(-_spawnRect.y / 2f, _spawnRect.y / 2f) + origin.y, origin.z);
                Piece piece = Instantiate(_piece.gameObject, position, Quaternion.identity, transform)
                    .GetComponent<Piece>();
                piece.transform.localScale = new Vector3(_pieceSize.x, _pieceSize.y, piece.transform.localScale.z);
                piece.transform.rotation = Quaternion.LookRotation(-Vector3.forward, -Vector3.up);
                piece.PuzzlePosition = piecePosition;
                piece.Puzzle = this;
                piece.Gates = gates;
                piece.Texture = _texture;
                piece.OnAssemble += Verify;
                piece.OnAssemble += _InvokeOnPieceAssemble;
                piece.gameObject.name = "Piece #" + x + ":" + y;
                _pieces.Add(piecePosition, piece);
            }
        }
    }

    private int[] GeneratePiece(Vector2Int position) {
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
