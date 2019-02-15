using UnityEngine;

namespace RayTracing
{
    public struct HitResult
    {
        public float T;
        public Vector3 Pos;
        public Vector3 Normal;
        public IMaterial Material;
    }

    public interface IHitable
    {
        bool Hit(Ray ray, float min, float max, ref HitResult hitResult);
    }
}
