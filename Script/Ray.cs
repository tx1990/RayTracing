using UnityEngine;

namespace RayTracing
{
    public class Ray
    {
        public Vector3 Original { set; get; }
        public Vector3 Direction { set; get; }
        public Vector3 NoramlDirection { get; private set; }

        public Ray(Vector3 o, Vector3 d)
        {
            Original = o;
            Direction = d;
            NoramlDirection = Direction.normalized;
        }

        public Vector3 GetPoint(float t)
        {
            return Original + t * Direction;
        }
    }
}
