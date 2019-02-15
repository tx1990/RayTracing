using UnityEngine;

namespace RayTracing
{
    public class Sphere : IHitable
    {
        public Vector3 Center { private set; get; }
        public float Radius { private set; get; }
        public IMaterial Material { private set; get; }

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public bool Hit(Ray ray, float min, float max, ref HitResult hitResult)
        {
            var oc = ray.Original - Center;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = 2f * Vector3.Dot(oc, ray.Direction);
            var c = Vector3.Dot(oc, oc) - Radius * Radius;
            var discriminant = b * b - 4 * a * c;
            if (discriminant > 0)
            {
                var result = (-b - Mathf.Sqrt(discriminant)) / (2f * a);
                if (result < max && result > min)
                {
                    var pos = ray.GetPoint(result);
                    hitResult = new HitResult
                    {
                        T = result,
                        Pos = pos,
                        Normal = (pos - Center).normalized,
                        Material = Material,
                    };
                    return true;
                }

                result = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
                if (result < max && result > min)
                {
                    var pos = ray.GetPoint(result);
                    hitResult = new HitResult
                    {
                        T = result,
                        Pos = pos,
                        Normal = (pos - Center).normalized,
                        Material = Material,
                    };
                    return true;
                }
            }

            return false;
        }
    }
}
