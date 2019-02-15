using UnityEngine;

namespace RayTracing
{
    public interface IMaterial
    {
        bool Scatter(Ray rayIn, HitResult result, out Color color, out Ray rayout);
    }

    public class Lambertian : IMaterial
    {
        private readonly Color m_diffuse;
        private readonly System.Random m_random;

        public Lambertian(Color diffuse)
        {
            m_diffuse = diffuse;
            m_random = new System.Random();
        }

        public bool Scatter(Ray rayIn, HitResult result, out Color color, out Ray rayout)
        {
            var target = result.Pos + result.Normal + GetRandomPointInUnitSphere();
            rayout = new Ray(result.Pos, target - result.Pos);
            color = m_diffuse;
            return true;
        }

        private Vector3 GetRandomPointInUnitSphere()
        {
            var p = 2f * new Vector3((float) m_random.NextDouble(), (float) m_random.NextDouble(),
                        (float) m_random.NextDouble()) - Vector3.one;
            p = p.normalized * (float) m_random.NextDouble();
            return p;
        }
    }

    public class Metal : IMaterial
    {
        private readonly Color m_specular;

        public Metal(Color specular)
        {
            m_specular = specular;
        }

        public bool Scatter(Ray rayIn, HitResult result, out Color color, out Ray rayout)
        {
            var reflect = Reflect(rayIn.NoramlDirection, result.Normal);
            rayout = new Ray(result.Pos, reflect);
            color = m_specular;
            return Vector3.Dot(rayout.Direction, result.Normal) > 0;
        }

        private Vector3 Reflect(Vector3 vin, Vector3 normal)
        {
            return vin - 2 * Vector3.Dot(vin, normal) * normal;
        }
    }
}