using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvertUtils  {

	public static Vector2Int Vector2ToVector2Int(Vector2 v)
    {
        return new Vector2Int((int)(v.x * 1000), (int)(v.y * 1000));
    }

    public static Vector3Int Vector3ToVector3Int(Vector3 v)
    {
        return new Vector3Int((int)(v.x * 1000), (int)(v.y * 1000), (int)(v.z * 1000));
    }

    public static Vector3 Vector3IntToVector3(Vector3Int v)
    {
        return new Vector3(v.x / 1000f, v.y / 1000f, v.z / 1000f);
    }

    public static Vector2 Vector2IntToVector2(Vector2Int v)
    {
        return new Vector2(v.x / 1000f, v.y / 1000f);
    }
}
