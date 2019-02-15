using System;
using UnityEngine;
using UnityEngine.UI;

namespace RayTracing
{
    public class RayTracing : MonoBehaviour
    {
        public RawImage Image;
        public int Sample = 10;
        public int MaxDepth = 1;
        public ComputeShader ComputeShader;

        private HitableList m_hitables;
        private MyCamera m_camera;
        private System.Random m_random;

        public struct SphereHit
        {
            public Vector3 Center;
            public float Radius;
            public Vector4 Color;
            public int MaterialType;
        }

        private ComputeBuffer m_hitBuffers;
        private ComputeBuffer m_randomBuffers;

        private void Start()
        {
            m_hitables = new HitableList();
            m_hitables.Add(new Sphere(new Vector3(0, 0, -1), 0.5f, new Lambertian(new Color(0.8f, 0.3f, 0.3f))));
            m_hitables.Add(new Sphere(new Vector3(0, -100.5f, -1), 100f, new Lambertian(new Color(0.8f, 0.8f, 0.0f))));
            //m_hitables.Add(new Sphere(new Vector3(1, 0, -1), 0.5f, new Metal(new Color(0.8f, 0.6f, 0.2f))));
            m_camera = new MyCamera(new Vector3(-2, -1, -1), new Vector3(4, 0, 0), new Vector3(0, 2, 0),
                new Vector3(0, 0, 0));
            m_random = new System.Random();

            //RefreshImage();
        }

        private Color GetColorForTestRay(Ray ray, int depth)
        {
            HitResult result = new HitResult();
            if (m_hitables.Hit(ray, 0, float.MaxValue, ref result))
            {
                Ray rayOut;
                Color color1;
                if (depth < MaxDepth && result.Material.Scatter(ray, result, out color1, out rayOut))
                {
                    var color2 = GetColorForTestRay(rayOut, depth + 1);
                    return color2 * color1;
                }

                return Color.black;
            }
            var t = 0.5f * (ray.NoramlDirection.y + 1);
            return Color.Lerp(new Color(1, 1, 1), new Color(0.5f, 0.7f, 1), t);
        }

        private Color[] CreateColorForTestRay(int width, int height)
        {
            var l = width * height;
            var colors = new Color[l];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    var col = Color.black;
                    for (int k = 0; k < Sample; k++)
                    {
                        var r = m_camera.GetRay((i + (float) m_random.NextDouble()) / width,
                            (j + (float) m_random.NextDouble()) / height);
                        col += GetColorForTestRay(r, 0);
                    }

                    colors[i + j * width] = col / Sample;
                }
            }

            return colors;
        }

        private void RefreshImage()
        {
            var delta = Image.rectTransform.sizeDelta;
            //Loom.RunAsync(() =>
            //{
            //    var colors = CreateColorForTestRay((int)delta.x, (int)delta.y);
            //    Loom.QueueOnMainThread(() =>
            //    {
            //        var texture = new Texture2D((int)delta.x, (int)delta.y);
            //        texture.SetPixels(colors);
            //        texture.Apply();
            //        Image.texture = texture;
            //    });
            //});

            var colors = CreateColorForTestRay((int)delta.x, (int)delta.y);
            var texture = new Texture2D((int)delta.x, (int)delta.y);
            texture.SetPixels(colors);
            texture.Apply();
            Image.texture = texture;

            //ComputeInShader();
        }

        private void ComputeInShader()
        {
            var delta = Image.rectTransform.sizeDelta;
            var texuture = new RenderTexture((int) delta.x, (int) delta.y, 0, RenderTextureFormat.ARGB32);
            texuture.enableRandomWrite = true;
            texuture.Create();

            var kernel = ComputeShader.FindKernel("CSMain");
            ComputeShader.SetTexture(kernel, "Result", texuture);

            var spheres = new SphereHit[2];
            spheres[0] = new SphereHit
            {
                Center = new Vector3(0, 0, -1),
                Radius = 0.5f,
                Color = new Vector4(0.8f, 0.3f, 0.3f, 1),
                MaterialType = 1,
            };
            spheres[1] = new SphereHit
            {
                Center = new Vector3(0, -100.5f, -1),
                Radius = 100f,
                Color = new Vector4(0.8f, 0.8f, 0.0f, 1),
                MaterialType = 1,
            };
            m_hitBuffers?.Release();
            m_hitBuffers = new ComputeBuffer(spheres.Length, 8 * sizeof(float) + sizeof(int));
            m_hitBuffers.SetData(spheres);
            ComputeShader.SetBuffer(kernel, "Spheres", m_hitBuffers);

            var length = (int) delta.x * (int) delta.y;
            var randoms = new float[length];
            m_randomBuffers?.Release();
            m_randomBuffers = new ComputeBuffer(length, sizeof(float));
            m_randomBuffers.SetData(randoms);
            ComputeShader.SetBuffer(kernel, "RandomValue", m_randomBuffers);

            ComputeShader.SetInt("HitCount", spheres.Length);
            ComputeShader.SetInt("Sample", Sample);
            ComputeShader.SetInt("MaxDepth", MaxDepth);

            ComputeShader.SetFloats("LowLeftCorner", -2, -1, -1);
            ComputeShader.SetFloats("Horizontal", 4, 0, 0);
            ComputeShader.SetFloats("Vertical", 0, 2, 0);
            ComputeShader.SetFloats("Original", 0, 0, 0);
            ComputeShader.SetFloat("Width", delta.x);
            ComputeShader.SetFloat("Height", delta.y);
            ComputeShader.SetFloat("MinDistance", 0);
            ComputeShader.SetFloat("MaxDistance", 10000);
            ComputeShader.SetInt("Seed", DateTime.Now.Millisecond);
            ComputeShader.SetFloat("A", 1664525);
            ComputeShader.SetFloat("C", 1013904223);
            ComputeShader.SetFloat("M", Mathf.Pow(2, 32));

            ComputeShader.Dispatch(kernel, (int) delta.x, (int) delta.y, 1);

            Image.texture = texuture;
        }

        void OnGUI()
        {
            if (GUILayout.Button("Direction"))
            {
                var delta = Image.rectTransform.sizeDelta;
                var colors = CreateColorForTestRay((int)delta.x, (int)delta.y);
                var texture = new Texture2D((int)delta.x, (int)delta.y);
                texture.SetPixels(colors);
                texture.Apply();
                Image.texture = texture;
            }
            if (GUILayout.Button("Loom"))
            {
                var delta = Image.rectTransform.sizeDelta;
                Loom.RunAsync(() =>
                {
                    var colors = CreateColorForTestRay((int)delta.x, (int)delta.y);
                    Loom.QueueOnMainThread(() =>
                    {
                        var texture = new Texture2D((int)delta.x, (int)delta.y);
                        texture.SetPixels(colors);
                        texture.Apply();
                        Image.texture = texture;
                    });
                });
            }
            if (GUILayout.Button("Compute Shader"))
            {
                ComputeInShader();
            }
        }

        void OnDestroy()
        {
            m_hitBuffers?.Release();
            m_randomBuffers?.Release();
        }
    }
}
