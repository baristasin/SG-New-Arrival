using UnityEngine;

public static class DistanceUtil
{
    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return (dx * dx) + (dz * dz);
    }
    
    public static float DistanceXZActual(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(DistanceXZ(a, b));
    }
}