using UnityEngine;

public static class Util {
    public static Vector2 Rotate(this Vector2 v, float degrees) {
        var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        var tx = v.x;
        var ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    
    public static Vector2 Rotated(this Vector2 v, float degrees) {
        var copyVector = new Vector2(v.x, v.y);
        return copyVector.Rotate(degrees);
    }

    public static Vector2Int ToIntVector(this Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }
    public static Vector2Int ToIntVector(this Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }
}