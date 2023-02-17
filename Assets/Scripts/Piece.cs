using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Piece : MonoBehaviour, IDraggable {
    [SerializeField] CornerPieceUVs _cornerInPrefab;
    [SerializeField] CornerPieceUVs _cornerStraightPrefab;
    [SerializeField] CornerPieceUVs _cornerOutPrefab;
    [SerializeField] UnityEvent _onAssemble = new UnityEvent();

    Puzzle _puzzle;
    Vector2Int _puzzlePosition;
    Texture _texture;
    int[] _gates = { 0, 0, 0, 0 };

    Dictionary<Direction, Piece> _neighbours = new Dictionary<Direction, Piece>();

    Color[] _debugColors = { Color.blue, Color.red, Color.yellow, Color.green };

    public Puzzle Puzzle { get => _puzzle; set => _puzzle = value; }
    public Vector2Int PuzzlePosition { get => _puzzlePosition; set => _puzzlePosition = value; }
    public Texture Texture { get => _texture; set => _texture = value; }
    public int[] Gates { get => _gates; set => _gates = value; }
    public DraggableParent DraggableParent { get => transform.parent.GetComponent<DraggableParent>(); set => transform.parent = value.transform; }
    public bool IsAssembled => transform.parent.GetComponent<IDraggable>() != null;

    public event UnityAction OnAssemble { add => _onAssemble.AddListener(value); remove => _onAssemble.RemoveListener(value); }

    private void Start() {
        InstantiateCorners();
    }

    private void InstantiateCorners() {
        for (int i = 0; i < 4; i++) {
            GameObject obj = Instantiate(GetCorner(_gates[i]), transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.LookRotation(-Vector3.forward, Tools.DirToV2((Direction)i));
            //obj.transform.localScale = _puzzle.PieceSize.ToV3(1f);
            obj.GetComponent<CornerPieceUVs>().SetUV(_texture, _puzzlePosition, _puzzle.PuzzleSize, (Direction)i);
            obj.name = ((Direction)i).ToString();
        }
    }

    private GameObject GetCorner(int gate) {
        switch (gate) {
            case -1:
                return _cornerInPrefab.gameObject;
            case 0:
                return _cornerStraightPrefab.gameObject;
            case 1:
                return _cornerOutPrefab.gameObject;
            default:
                return null;
        }
    }

    public void OnPickup() {
        if (IsAssembled) {
            DragManager.Pickup(transform.parent.gameObject);
        }
    }

    public void OnDrop() {
        for (int i = 0; i < 4; ++i) {
            Direction direction = (Direction)i;
            Vector2 directionV2 = Tools.DirToV2(direction);
            if (_neighbours.ContainsKey(direction)) { continue; }

            float rayDistance = i % 2 == 0 ? _puzzle.PieceSize.y : _puzzle.PieceSize.x;

            Vector3 rayDirection = Quaternion.Inverse(transform.rotation) * -directionV2;

            Debug.DrawRay(transform.position, rayDirection * rayDistance, _debugColors[i], 5f);
            if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, rayDistance)) {
                if (hit.collider.gameObject.TryGetComponent(out Piece piece)) {
                    if (!IsNextTo(piece, direction)) { continue; }
                    TeleportNextTo(piece, direction);
                    Assemble(piece);
                    _onAssemble?.Invoke();

                    AddNeighbours(direction, piece);
                    piece.AddNeighbours(direction.Inverse(), this);
                }
            }
        }
    }

    private void TeleportNextTo(Piece piece, Direction direction) {
        if (IsAssembled) {
            Vector3 offset = transform.parent.position - transform.position;
            transform.parent.position = piece.transform.position + (transform.rotation * Tools.DirToV2(direction) * _puzzle.PieceSize).ToV3() + offset;
            return;
        }
        transform.position = piece.transform.position + (transform.rotation * Tools.DirToV2(direction) * _puzzle.PieceSize).ToV3();
    }

    public void Assemble(Piece piece) {
        if (piece == null) { return; }

        if (piece.IsAssembled) {
            if (IsAssembled) {
                if (DraggableParent == piece.DraggableParent) {
                    return;
                }
                piece.DraggableParent.Merge(DraggableParent);
            } else {
                DraggableParent = piece.DraggableParent;
            }
        } else {
            if (!IsAssembled) {
                DraggableParent parent = Instantiate(_puzzle.AssembledParent, transform.position, Quaternion.identity, transform.parent).GetComponent<DraggableParent>();
                DraggableParent = parent;
                piece.DraggableParent = parent;
            } else {
                piece.DraggableParent = DraggableParent;
            }
        }
    }

    public bool IsNextTo(Piece piece, Direction direction) {
        return PuzzlePosition + Tools.DirToV2(direction).UpIsDown() == piece.PuzzlePosition;
    }

    public bool InPlace() {
        for (int i = 0; i < 4; i++) {
            if (!_neighbours.ContainsKey((Direction)i) && _gates[i] != 0) {
                return false;
            }
        }
        return true;
    }

    public void AddNeighbours(Direction direction, Piece piece) {
        _neighbours.Add(direction, piece);
    }
}
