using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle Informations", menuName = "Scriptable Objects/Informations/Puzzle Informations")]
public class PuzzleInformation : ScriptableObject {
    Texture2D _texture = null;
    Vector2Int _size = Vector2Int.zero;
    Vector2 _pieceScale = Vector2.zero;

    public Texture2D Texture { get => _texture; set => _texture = value; }
    public Vector2Int Size { get => _size; set => _size = value; }
    public Vector2 PieceScale { get => _pieceScale; set => _pieceScale = value; }
}
