using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Piece : MonoBehaviour, IDraggable {
    [SerializeField] Color[] _debugColors;
    [SerializeField] UnityEvent _onAssemble = new UnityEvent();

    Puzzle _puzzle;
    Vector2Int _puzzlePosition;
    Dictionary<Direction, Piece> _neighbours = new Dictionary<Direction, Piece>();

    public Puzzle Puzzle { get => _puzzle; set => _puzzle = value; }
    public Vector2Int PuzzlePosition { get => _puzzlePosition; set => _puzzlePosition = value; }
    public DraggableParent DraggableParent { get => transform.parent.GetComponent<DraggableParent>(); set => transform.parent = value.transform; }
    public bool IsAssembled => transform.parent.GetComponent<IDraggable>() != null;
    public event UnityAction OnAssemble { add => _onAssemble.AddListener(value); remove => _onAssemble.RemoveListener(value); }

    public void OnPickup() {
        if (IsAssembled) {
            DragManager.Pickup(transform.parent.gameObject);
        }
    }

    public void OnDrop() {
        for (int i = 0; i < 4; ++i) {
            Direction direction = (Direction)i;
            if (_neighbours.ContainsKey(direction)) { continue; }

            float rayDistance = i % 2 == 0 ? _puzzle.PieceSize.y : _puzzle.PieceSize.x;

            Debug.DrawRay(transform.position, Tools.DirToV2(direction) * rayDistance, _debugColors[i], 5f);
            if (Physics.Raycast(transform.position, Tools.DirToV2(direction), out RaycastHit hit, rayDistance)) {
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
            transform.parent.position = piece.transform.position + (-Tools.DirToV2(direction) * _puzzle.PieceSize).ToV3() + offset;
            return;
        }
        transform.position = piece.transform.position + (-Tools.DirToV2(direction) * _puzzle.PieceSize).ToV3();
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
        return PuzzlePosition + Tools.DirToV2(direction) == piece.PuzzlePosition;
    }

    public void AddNeighbours(Direction direction, Piece piece) {
        _neighbours.Add(direction, piece);
    }
}
