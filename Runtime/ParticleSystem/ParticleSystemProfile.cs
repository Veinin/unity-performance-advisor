using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor;

namespace UPA
{
    /// <summary>
    /// 粒子系统性能分析器
    /// </summary>
    public class ParticleSystemProfile : MonoBehaviour
    {
        public static event Action<ParticleSystemResult> OnProfilingComplete;

        private string m_Path;

        private float m_Duration;
        private float m_MaxDuration;
        private float m_CurrentTime;

        private Camera m_Camera;
        private RenderTexture m_RenderTexture;
        private ParticleSystem[] m_ParticleSystems;
        private ParticleSystemResult m_ParticleSystemResult;

        public string path
        {
            set { m_Path = value; }
        }

        public float maxDuration
        {
            set { m_MaxDuration = value == 0 ? 3f : value + 0.5f; }
            get { return m_MaxDuration; }
        }

        private void Awake()
        {
            m_Camera = Camera.main;
            m_Camera.SetReplacementShader(Shader.Find("XunShan/Profiler/OverDraw"), "");
            m_RenderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
            m_ParticleSystemResult = new ParticleSystemResult();
        }

        private void LateUpdate()
        {
            RecordParticleCount();
            RecordDrawCall();
            RecordFrameTime();
            RecordMemoryUsage();
            RecordOverDraw();

            m_Duration += Time.deltaTime;
            m_CurrentTime = Time.realtimeSinceStartup;

            if (m_Duration > m_MaxDuration)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void OnDestroy()
        {
            m_ParticleSystemResult.path = m_Path;
            m_ParticleSystemResult.Summarize();
            
            OnProfilingComplete(m_ParticleSystemResult);
        }

        private void RecordParticleCount()
        {
            m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
            if (m_ParticleSystems.Length == 0)
            {
                return;
            }

            var compCount = 0;
            var particleCount = 0;
            foreach (var ps in m_ParticleSystems)
            {
                particleCount += ps.particleCount;
                if (ps.isPlaying)
                {
                    compCount++;
                }
            }
            m_ParticleSystemResult.particleCompCounts.Add(compCount);
            m_ParticleSystemResult.particleCounts.Add(particleCount);
        }

        private void RecordDrawCall()
        {
            m_ParticleSystemResult.drawCalls.Add(UnityStats.batches / 2);
        }

        private void RecordFrameTime()
        {
            m_ParticleSystemResult.frameTimes.Add(Time.realtimeSinceStartup - m_CurrentTime);
        }

        private void RecordMemoryUsage()
        {
            var textures = new HashSet<Texture>();
            int sumSize = 0;

            var rendererList = gameObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (var item in rendererList)
            {
                if (item.sharedMaterial)
                {
                    Texture texture = item.sharedMaterial.mainTexture;
                    if (texture && !textures.Contains(texture))
                    {
                        textures.Add(texture);
                        sumSize += (int)Profiler.GetRuntimeMemorySizeLong(texture);
                    }
                }
            }
            m_ParticleSystemResult.memoryUsages.Add(sumSize);
            m_ParticleSystemResult.textureUseages.Add(textures.Count);
        }

        private void RecordOverDraw()
        {
            RenderTexture currTexture = RenderTexture.active;

            m_Camera.targetTexture = m_RenderTexture;
            m_Camera.Render();

            RenderTexture.active = m_RenderTexture;

            var texture = new Texture2D(m_RenderTexture.width, m_RenderTexture.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, m_RenderTexture.width, m_RenderTexture.width), 0, 0);

            UpdateOverDrawData(texture);

            RenderTexture.active = currTexture;
            m_Camera.targetTexture = null;
            Texture2D.DestroyImmediate(texture);
            m_RenderTexture.Release();
        }

        private void UpdateOverDrawData(Texture2D texture)
        {
            var pixelTotal = 0;
            var pixelActualTotal = 0;

            int index = 0;
            var pixels = texture.GetPixels();

            for (var x = 0; x < texture.width; x++)
            {
                for (var y = 0; y < texture.height; y++)
                {
                    float r = pixels[index].r;
                    float g = pixels[index].g;
                    float b = pixels[index].b;

                    var isEmptyPixel = r == 0 && g == 0 && b == 0;
                    if (!isEmptyPixel)
                    {
                        pixelTotal++;
                    }

                    pixelActualTotal += GetDrawPixelTimes(g);
                    index++;
                }
            }

            m_ParticleSystemResult.pixelDraws.UpdateData(pixelTotal, pixelActualTotal);
        }

        private int GetDrawPixelTimes(float g)
        {
            //在 OverDraw.Shader 中像素每渲染一次，g 值就会叠加 0.04
            return Convert.ToInt32(g / 0.04);
        }
    }
}