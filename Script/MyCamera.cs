using UnityEngine;

namespace RayTracing
{
    public class MyCamera
    {
        private readonly Vector3 m_lowLeftCorner;
        private readonly Vector3 m_horizontal;
        private readonly Vector3 m_vertical;
        private readonly Vector3 m_original;

        public MyCamera(Vector3 lowLeftCorner, Vector3 horizontal, Vector3 vertical, Vector3 original)
        {
            m_lowLeftCorner = lowLeftCorner;
            m_horizontal = horizontal;
            m_vertical = vertical;
            m_original = original;
        }

        public Ray GetRay(float u, float v)
        {
            return new Ray(m_original, m_lowLeftCorner + m_horizontal * u + m_vertical * v - m_original);
        }
    }
}
