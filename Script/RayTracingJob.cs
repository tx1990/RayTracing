using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RayTracing
{
    public struct RayTracingAllInJob : IJobParallelFor
    {
        public NativeArray<Color> Colors;
        public int Width;
        public int Height;
        public MyCamera Camera;
        public Sphere Hitable;
        public MyRandom Random;
        public int Sample;

        public void Execute(int index)
        {
            var j = index / Width;
            var i = index % Width;

            var c = Color.black;
            for (int k = 0; k < Sample; k++)
            {
                var r = Camera.GetRay((i + Random.Range01()) / Width, (j + Random.Range01()) / Height);
                c += GetColorForTestRay(r);
            }
            Colors[index] = c/Sample;
        }

        private Color GetColorForTestRay(Ray ray)
        {
            HitResult result = new HitResult();
            if (Hitable.Hit(ray, 0, float.MaxValue, ref result))
            {
                var target = result.Pos + result.Normal + GetRandomPointInUnitSphere();
                return 0.5f * GetColorForTestRay(new Ray(result.Pos, target - result.Pos));
            }
            var t = 0.5f * (ray.NoramlDirection.y + 1);
            return Color.Lerp(new Color(1, 1, 1), new Color(0.5f, 0.7f, 1), t);
        }

        private Vector3 GetRandomPointInUnitSphere()
        {
            var p = new Vector3(Random.Range01(), Random.Range01(), Random.Range01());
            p = p * 2 - Vector3.one;
            p = p.normalized * Random.Range01();
            return p;
        }
    }

    public struct RayTracingJob : IJobParallelFor
    {
        public NativeArray<Color> Colors;
        public int Width;
        public int Height;
        public MyCamera Camera;
        public Sphere Hitable;
        public MyRandom Random;

        public void Execute(int index)
        {
            var j = index / Width;
            var i = index % Width;

            var r = Camera.GetRay((i + Random.Range01()) / Width, (j + Random.Range01()) / Height);
            Colors[index] = GetColorForTestRay(r);
        }

        private Color GetColorForTestRay(Ray ray)
        {
            HitResult result = new HitResult();
            if (Hitable.Hit(ray, 0, float.MaxValue, ref result))
            {
                var target = result.Pos + result.Normal + GetRandomPointInUnitSphere();
                return 0.5f * GetColorForTestRay(new Ray(result.Pos, target - result.Pos));
            }
            var t = 0.5f * (ray.NoramlDirection.y + 1);
            return Color.Lerp(new Color(1, 1, 1), new Color(0.5f, 0.7f, 1), t);
        }

        private Vector3 GetRandomPointInUnitSphere()
        {
            var p = new Vector3(Random.Range01(), Random.Range01(), Random.Range01());
            p = p * 2 - Vector3.one;
            p = p.normalized * Random.Range01();
            return p;
        }
    }

    public struct ColorAddJob : IJobParallelFor
    {
        public NativeArray<Color> Colors1;
        public NativeArray<Color> Colors2;

        public void Execute(int index)
        {
            Colors2[index] += Colors1[index];
        }
    }

    public struct ColorResultJob : IJobParallelFor
    {
        public NativeArray<Color> Colors;
        public int Sample;

        public void Execute(int index)
        {
            Colors[index] /= Sample;
        }
    }
}
