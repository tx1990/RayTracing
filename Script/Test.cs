using System;
using RayTracing;
using UnityEngine;

public class Test : MonoBehaviour
{
    [ContextMenu("RandomTest")]
    private void RandomTest()
    {
        var r = new MyRandom(DateTime.Now.Millisecond);
        for (int i = 0; i < 100; i++)
        {
            Debug.LogWarning(r.Range01());
        }
    }
}
