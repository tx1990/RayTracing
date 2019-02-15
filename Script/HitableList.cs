using System.Collections.Generic;
using UnityEngine;

namespace RayTracing
{
    public class HitableList
    {
        private List<IHitable> m_hitables;

        public HitableList(params IHitable[] hitable)
        {
            m_hitables = new List<IHitable>(hitable);
        }

        public void Add(IHitable hitable)
        {
            m_hitables.Add(hitable);
        }

        public void Remove(IHitable hitable)
        {
            m_hitables.Remove(hitable);
        }

        public bool Hit(Ray ray, float min, float max, ref HitResult hitResult)
        {
            var closest = max;
            var hit = false;
            for (int i = 0; i < m_hitables.Count; i++)
            {
                if (m_hitables[i].Hit(ray, min, closest, ref hitResult))
                {
                    closest = hitResult.T;
                    hit = true;
                }
            }

            return hit;
        }
    }
}
