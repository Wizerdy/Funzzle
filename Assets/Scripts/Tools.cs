using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    UP, RIGHT, DOWN, LEFT
}

public static class Tools {
    public static Vector3 ToV3(this Vector2 vector) {
        return new Vector3(vector.x, vector.y, 0);
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

    public static Direction Inverse(this Direction direction) {
        return (Direction)(((int)direction + 2) % 4);
    }
}
