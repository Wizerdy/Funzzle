using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CornerPieceUVs : MonoBehaviour {
    [SerializeField] Texture d_texture;
    [SerializeField] Vector2 d_piecePosition = Vector2.zero;
    [SerializeField] Vector2 d_puzzleSize = new Vector2(10f, 10f);
    [SerializeField] Direction d_direction;
    Vector2 _piecePosition = Vector2.one * -1f;
    Vector2 _puzzleSize = Vector2.one * -1f;
    Direction _direction;

    //void OnValidate() {
    //    SetUV(d_texture, d_piecePosition, d_puzzleSize, d_direction);
    //}

    public void SetUV(Texture texture, Vector2 piecePosition, Vector2 puzzleSize, Direction direction) {
        _piecePosition = piecePosition;
        _puzzleSize = puzzleSize;
        _direction = direction;

        Material mat = GetComponent<Renderer>().material;
        if (Application.isPlaying) {
            mat = GetComponent<Renderer>().sharedMaterial;
        }

        if (mat == null) { return; }
        if (puzzleSize.x == 0 || puzzleSize.y == 0) { return; }

        Vector2 position = piecePosition;
        Vector2 uvSize = Vector2.one / puzzleSize;
        float angle = Angle(direction);

        mat.SetTexture("_Texture", texture);

        Vector2 correction = Tools.DirToV2(direction) * 0.5f * new Vector2(1f, -1f);
        //Debug.Log(name + " : " + correction);

        Matrix4x4 matrix = Matrix4x4.TRS(puzzleSize / 2f, Quaternion.AngleAxis(-angle, Vector3.forward), Vector2.one);
        position = matrix.MultiplyVector((position + correction).ToV3() - matrix.GetPosition());
        position += new Vector2(matrix.GetPosition().x, matrix.GetPosition().y);

        mat.SetVector("_UVPosition", (position - WeirdDirections(direction)) / puzzleSize);

        //uvSize = matrix.MultiplyVector(uvSize);
        mat.SetVector("_UVSize", uvSize);

        mat.SetFloat("_UVRotation", angle);

        Debug.Log(name + " : Pos:" + position + "; MPos:" + matrix.GetPosition() + "; UV:" + uvSize);
    }

    float Angle(Direction direction) {
        return (((int)direction * 90f) + 90f) % 360f;
    }

    Vector2 WeirdDirections(Direction direction) {
        switch (direction) {
            case Direction.UP:
                return new Vector2(0f, 1f);
            case Direction.RIGHT:
                return new Vector2(1f, 1f);
            case Direction.DOWN:
                return new Vector2(1f, 0f);
            case Direction.LEFT:
                return new Vector2(0f, 0f);
            default:
                return Vector2.zero;
        }
    }
}
