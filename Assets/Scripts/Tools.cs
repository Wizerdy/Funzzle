using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    UP, RIGHT, DOWN, LEFT
}

public static class Tools {
    public static Vector3 ToV3(this Vector2 vector, float value = 0f) {
        return new Vector3(vector.x, vector.y, value);
    }

    public static Vector2Int ToV2I(this Vector2 vector) {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }

    public static Vector2 DirToV2(Direction direction) {
        switch (direction) {
            case Direction.UP:
                return Vector2.up;
            case Direction.RIGHT:
                return Vector2.right;
            case Direction.DOWN:
                return Vector2.down;
            case Direction.LEFT:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }

    public static Direction UpIsDown(this Direction direction) {
        return (int)direction % 2 == 0 ? direction.Inverse() : direction;
    }

    public static Vector2Int DirToV2I(Direction direction) {
        return DirToV2(direction).ToV2I();
    }

    public static Vector2 UpIsDown(this Vector2 vector) {
        return new Vector2(vector.x, -vector.y);
    }

    public static Direction Inverse(this Direction direction) {
        return (Direction)(((int)direction + 2) % 4);
    }

    public static string Print<T>(this T[] value) {
        string output = "[ ";
        if (value.Length == 0) {
            output += "]";
            return output;
        }
        output += "{ ";
        for (int i = 0; i < value.Length - 1; i++) {
            output += value[i] + " }, { ";
        }
        output += value[value.Length - 1] + " } ]";
        return output;
    }
}
