using UnityEngine;

public static class DistanceUtil
{
    /// <summary>
    /// Returns distance value not squared, based on X-Z (Y is not considered)
    /// </summary>
    public static float DistanceXZ(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return (dx * dx) + (dz * dz);
    }

    /// <summary>
    /// Squared real distance value, heavy on CPU so, careful usage
    /// </summary>
    public static float DistanceXZActual(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(DistanceXZ(a, b));
    }
}