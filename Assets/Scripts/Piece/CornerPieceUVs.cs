using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

[DisallowMultipleComponent]
public class CornerPieceUVs : MonoBehaviour {
    [SerializeField] Texture d_texture;
    [SerializeField] Vector2 d_piecePosition = Vector2.zero;
    [SerializeField] Vector2Int d_puzzleSize = new Vector2Int(10, 10);
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

        Material mat;
        if (!Application.isPlaying) {
            mat = GetComponent<Renderer>().sharedMaterial;
        } else {
            mat = GetComponent<Renderer>().material;
        }

        if (mat == null) { return; }
        if (puzzleSize.x == 0 || puzzleSize.y == 0) { return; }

        Vector2 position = piecePosition;
        Vector2 uvSize = Vector2.one / puzzleSize;

        if ((int)direction % 2 == 0) {
            float temp = uvSize.x;
            uvSize.x = uvSize.y;
            uvSize.y = temp;
        }

        float angle = Angle(direction);

        mat.SetTexture("_Texture", texture);

        Vector2 correction = Tools.DirToV2(direction) * 0.5f * uvSize * new Vector2(1f, -1f);

        if ((int)direction % 2 == 0) {
            correction.y = (uvSize.y / -2f + uvSize.x / 2f) - Tools.DirToV2(direction).y * (uvSize.x / 2f);
            correction.x = ((uvSize.y < uvSize.x ? -uvSize.y : uvSize.x) / 2f) * (Mathf.Max(puzzleSize.x, puzzleSize.y) / Mathf.Min(puzzleSize.x, puzzleSize.y) - 1f);
        }

        //Debug.Log(name + " : " + correction);

        //Matrix4x4 matrix = Matrix4x4.TRS(puzzleSize / 2f, Quaternion.AngleAxis(-angle, Vector3.forward), Vector2.one);
        //position = matrix.MultiplyVector((position + correction).ToV3() - matrix.GetPosition());
        //position += new Vector2(matrix.GetPosition().x, matrix.GetPosition().y);

        //mat.SetVector("_UVPosition", (position - WeirdDirections(direction)) / puzzleSize);
        mat.SetVector("_UVPosition", position / puzzleSize + correction);

        //uvSize = matrix.MultiplyVector(uvSize);
        mat.SetVector("_UVSize", uvSize);

        mat.SetFloat("_UVRotation", angle);

        //Debug.Log(name + " : Pos:" + position + "; MPos:" + matrix.GetPosition() + "; UV:" + uvSize);
    }

    float Angle(Direction direction) {
        return (((int)direction * 90f) + 90f) % 360f;
    }
}
