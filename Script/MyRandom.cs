using System;

namespace RayTracing
{
    public class MyRandom
    {
        private readonly float a;
        private readonly float c;
        private readonly float m;
        private float x;

        public MyRandom(float seed)
        {
            x = seed;
            m = (float) Math.Pow(2, 32);
            a = 1664525;
            c = 1013904223;
        }

        public float Range01()
        {
            x = (a * x + c) % m;
            return x / m;
        }
    }
}
