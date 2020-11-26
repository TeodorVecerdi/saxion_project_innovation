using UnityEngine;

public static class MathHelper {
    public static Vector3 SphericalToCartesian(float radius, float heading, float pitch) {
        var dirX = radius * Mathf.Sin(pitch) * Mathf.Cos(heading);
        var dirZ = radius * Mathf.Sin(pitch) * Mathf.Sin(heading);
        var dirY = radius * Mathf.Cos(pitch);
        return new Vector3(dirX, dirY, dirZ);
    }

    public static (float radius, float heading, float pitch) CartesianToSpherical(Vector3 cartesian) {
        var radius = cartesian.magnitude;
        var heading = Mathf.Atan(cartesian.z / cartesian.x);
        var pitch = Mathf.Acos(cartesian.y / radius);
        return (radius, heading, pitch);
    }

    public static float WrapPI(float theta) {
        if (Mathf.Abs(theta) <= Mathf.PI) {
            var twoPI = Mathf.PI * Mathf.PI;
            var revolutions = Mathf.Floor((theta + Mathf.PI) * (1f / twoPI));
            theta -= revolutions * twoPI;
        }

        return theta;
    }

    public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static Vector3 Map(this Vector3 value, float fromSource, float toSource, float fromTarget, float toTarget) {
        return new Vector3(value.x.Map(fromSource, toSource, fromTarget, toTarget), value.y.Map(fromSource, toSource, fromTarget, toTarget), value.z.Map(fromSource, toSource, fromTarget, toTarget));
    }

    public static Vector3 Map(this Vector3 value, Vector3 fromSource, Vector3 toSource, Vector3 fromTarget, Vector3 toTarget) {
        return new Vector3(value.x.Map(fromSource.x, toSource.x, fromTarget.x, toTarget.x), value.y.Map(fromSource.y, toSource.y, fromTarget.y, toTarget.y), value.z.Map(fromSource.z, toSource.z, fromTarget.z, toTarget.z));
    }

    public static Vector4 V4(this Vector3 vector, float w = 0) {
        return new Vector4(vector.x, vector.y, vector.z, w);
    }
    
    public static Vector4 V4(this Vector2 vector, float z = 0, float w = 0) {
        return new Vector4(vector.x, vector.y, z, w);
    }

    public static Vector3 V3(this Vector4 vector) {
        return new Vector3(vector.x, vector.y, vector.z);
    }
}