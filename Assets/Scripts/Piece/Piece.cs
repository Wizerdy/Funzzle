using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine.BetterEvents;
using UnityEngine.Events;
using ToolsBoxEngine;

[SelectionBase]
public class Piece : MonoBehaviour, IDraggable {
    [Header("System")]
    [SerializeField] CornerPieceUVs _cornerInPrefab;
    [SerializeField] CornerPieceUVs _cornerStraightPrefab;
    [SerializeField] CornerPieceUVs _cornerOutPrefab;
    [SerializeField] LayerMask _pieceDetectionLayer;
    [Header("Parameters")]
    [SerializeField, Range(0f, 1f)] float _detectionDistance = 1f;
    [Space]
    [SerializeField] BetterEvent _onTake = new BetterEvent();
    [SerializeField] BetterEvent _onRelease = new BetterEvent();
    [SerializeField] BetterEvent _onAssemble = new BetterEvent();

    bool _init = false;
    Rigidbody _rb;
    Puzzle _puzzle;
    Vector2Int _puzzlePosition;
    Texture _texture;
    int[] _gates = { 0, 0, 0, 0 };

    Dictionary<Direction, Piece> _neighbours = new Dictionary<Direction, Piece>();

    Color[] _debugColors = { Color.blue, Color.red, Color.yellow, Color.green };

    public GameObject GameObject => gameObject;
    public Rigidbody Rigidbody => DraggableParent ? DraggableParent.GetComponent<Rigidbody>() : _rb;
    public Puzzle Puzzle { get => _puzzle; set => _puzzle = value; }
    public Vector2Int PuzzlePosition { get => _puzzlePosition; set => _puzzlePosition = value; }
    public Texture Texture { get => _texture; set => _texture = value; }
    public int[] Gates { get => _gates; set => _gates = value; }
    public DraggableParent DraggableParent { get => transform.parent.GetComponent<DraggableParent>(); set => transform.parent = value.transform; }
    public bool IsAssembled => transform.parent.GetComponent<IDraggable>() != null;

    public event UnityAction OnTake { add => _onTake += value; remove => _onTake += value; }
    public event UnityAction OnRelease { add => _onRelease += value; remove => _onRelease += value; }
    public event UnityAction OnAssemble { add => _onAssemble += value; remove => _onAssemble += value; }

    private void Start() {
        Init();
    }

    public void Init() {
        if (_init) { return; }
        _rb = GetComponent<Rigidbody>();
        OnAssemble += RemoveRigidbody;
        InstantiateCorners();
    }

    private void InstantiateCorners() {
        for (int i = 0; i < 4; i++) {
            try {
                GameObject obj = Instantiate(GetCorner(_gates[i]), transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.LookRotation(-Vector3.forward, Tools.DirToV2((Direction)i));
                //obj.transform.localScale = _puzzle.PieceSize.ToV3(1f);
                obj.GetComponent<CornerPieceUVs>().SetUV(_texture, _puzzlePosition, _puzzle.PuzzleSize, (Direction)i);
                obj.name = ((Direction)i).ToString();
            } catch (System.Exception e) {
                Debug.LogError(e);
            }
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
        transform.rotation = Quaternion.LookRotation(-Vector3.forward, -Vector3.up);
        if (IsAssembled) {
            DragManager.Pickup(transform.parent.gameObject.GetComponentInRoot<IDraggable>());
        }
        _onTake.Invoke();
    }

    public void OnDrop() {
        for (int i = 0; i < 4; ++i) {
            Direction direction = (Direction)i;
            Vector2 directionV2 = Tools.DirToV2(direction);
            if (_neighbours.ContainsKey(direction)) { continue; }

            float rayDistance = (i % 2 == 0 ? _puzzle.PieceSize.y : _puzzle.PieceSize.x) * _detectionDistance;

            Vector3 rayDirection = Quaternion.Inverse(transform.rotation) * -directionV2;

            Debug.DrawRay(transform.position.Override(0f, Axis.Z), rayDirection * rayDistance, _debugColors[i], 5f);

            RaycastHit? hit = null;
            RaycastHit[] hits = Physics.RaycastAll(transform.position.Override(0f, Axis.Z), rayDirection, rayDistance, _pieceDetectionLayer, QueryTriggerInteraction.Collide);
            for (int j = 0; j < hits.Length; j++) {
                if (hits[j].collider.gameObject.GetRoot() == gameObject) { continue; }
                hit = hits[j];
            }

            if (!hit.HasValue) { continue; }

            if (hit.Value.collider.gameObject.TryGetRootComponent(out Piece piece)) {
                if (!IsNextTo(piece, direction)) { continue; }

                if (hit.Value.collider.transform.rotation != transform.rotation) {
                    Debug.Log("Not Same Rotation");
                    continue;
                }

                TeleportNextTo(piece, direction);
                Assemble(piece);

                AddNeighbours(direction, piece);
                piece.AddNeighbours(direction.Inverse(), this);

                _onAssemble.Invoke();
                piece._onAssemble.Invoke();
            }
        }
        _onRelease.Invoke();
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
        if (piece.IsAssembled && IsAssembled && piece.DraggableParent == DraggableParent) {
                return;
        }

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

    private void RemoveRigidbody() {
        Destroy(_rb);
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

    public void ApplyPhysicState(PhysicState state) {
        if (Rigidbody == null) { Debug.LogError("Rigidbody not set !"); return; }

        Rigidbody.velocity = state.velocity;
        Rigidbody.angularVelocity = state.angularVelocity;
        Rigidbody.rotation = state.rotation;
        Rigidbody.position = state.position;
    }
}
