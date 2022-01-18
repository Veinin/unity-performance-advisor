using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    public class ShaderCheckResult
    {
        public int VariantCount;
        public int TexEnvCount;
        public string[] GlobalKeywords;
        public string[] LocalKeywords;

        public int GlobakKeywordCount
        {
            get { return GlobalKeywords.Length; }
        }

        public int LocalKeywordCount
        {
            get { return LocalKeywords.Length; }
        }

        public int KeywordCount
        {
            get { return LocalKeywordCount + GlobakKeywordCount; }
        }
    }

    public static class AnalysistUtility
    {
        public static string GetMemorySize(float bits)
        {
            var bytes = bits / 1024;
            if (bytes >= 1024)
            {
                return (bytes / 1024).ToString("f2") + "MB";
            }
            else
            {
                return bytes.ToString("f1") + "KB";
            }
        }

        #region Mesh Utility

        public static int GetMeshTris(GameObject go)
        {
            var tris = 0;

            var filter = go.GetComponent<MeshFilter>();
            if (filter != null)
            {
                tris += filter.sharedMesh.triangles.Length / 3;
            }

            var filters = go.GetComponentsInChildren<MeshFilter>();
            foreach(var f in filters)
            {
                tris += f.sharedMesh.triangles.Length / 3;
            }

            var meshRenderer = go.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                tris += meshRenderer.sharedMesh.triangles.Length / 3;
            }

            var meshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(var r in meshRenderers)
            {
                tris += r.sharedMesh.triangles.Length / 3;
            }

            return tris;
        }

        public static int GetMeshVerts(GameObject go)
        {
            var verts = 0;

            var filter = go.GetComponent<MeshFilter>();
            if (filter != null)
            {
                verts += filter.sharedMesh.vertexCount;
            }

            var filters = go.GetComponentsInChildren<MeshFilter>();
            foreach(var f in filters)
            {
                verts += f.sharedMesh.vertexCount;
            }

            var meshRenderer = go.GetComponent<SkinnedMeshRenderer>();
            if (meshRenderer != null)
            {
                verts += meshRenderer.sharedMesh.vertexCount;
            }

            var meshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(var r in meshRenderers)
            {
                verts += r.sharedMesh.vertexCount;
            }

            return verts;
        }
        
        #endregion

        #region Shader Utility

        private static Dictionary<string, ShaderCheckResult> ShaderResults = new Dictionary<string, ShaderCheckResult>();

        public static ShaderCheckResult GetShaderCheckResult(Shader shader)
        {
            var name = shader.name;
            if (ShaderResults.ContainsKey(name))
            {
                return ShaderResults[name];
            }
            else
            {
                var result = new ShaderCheckResult();
                result.TexEnvCount = GetShaderTexEnvCount(shader);
                result.VariantCount = GetShaderVariantCount(shader);
                result.GlobalKeywords = GetShaderGlobalKeywords(shader);
                result.LocalKeywords = GetShaderLocalKeywords(shader);

                ShaderResults[name] = result;

                return result;
            }
        }

        private static MethodInfo GetVariantCount;
        private static MethodInfo GetGlobalKeywords;
        private static MethodInfo GetLocalKeywords;

        public static int GetShaderVariantCount(Shader shader)
        {
            if (GetVariantCount == null)
            {
                GetVariantCount = GetMethodInfo(typeof(ShaderUtil), "GetVariantCount");
            }
            var variantCount = GetVariantCount.Invoke(null, new System.Object[] { shader, true });
            return int.Parse(variantCount.ToString());
        }

        public static string[] GetShaderGlobalKeywords(Shader shader)
        {
            if (GetGlobalKeywords == null)
            {
                GetGlobalKeywords = GetMethodInfo(typeof(ShaderUtil), "GetShaderGlobalKeywords");
            }
            return GetGlobalKeywords.Invoke(null, new System.Object[] { shader }) as string[];
        }

        public static string[] GetShaderLocalKeywords(Shader shader)
        {
            if (GetLocalKeywords == null)
            {
                GetLocalKeywords = GetMethodInfo(typeof(ShaderUtil), "GetShaderLocalKeywords");
            }
            return GetLocalKeywords.Invoke(null, new System.Object[] { shader }) as string[];
        }

        public static int GetShaderTexEnvCount(Shader shader)
        {
            var count = 0;
            for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    count++;
                }
            }
            return count;
        }

        private static MethodInfo GetMethodInfo(Type type, string method)
        {
            return type.GetMethod(method, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        #endregion

        public static float GetParticleSystemDuration(GameObject go)
        {
            var pts = go.transform.GetComponentsInChildren<ParticleSystem>(true);
            float maxDuration = 0f;
            foreach (var p in pts)
            {
                if (!p.emission.enabled)
                {
                    continue;
                }

                if (p.main.loop)
                {
                    return 5;
                }

                float duration = 0f;
                if (p.emission.rateOverTime.constant <= 0)
                {
                    duration = p.main.startDelay.constant + p.main.startLifetime.constant;
                }
                else
                {
                    duration = p.main.startDelay.constant + Mathf.Max(p.main.duration, p.main.startLifetime.constant);
                }
                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }
            }
            return maxDuration;
        }

        public static bool CheckSolidColor(Texture texture)
        {
            bool isSolidColor = true;
            int rtWidth = texture.width;
            int rtHeight = texture.height;

            if (rtWidth >= 128)
            {
                rtWidth = rtWidth / 8;
            }

            if (rtHeight >= 128)
            {
                rtHeight = rtHeight / 8;
            }

            var rt = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            Graphics.Blit(texture, rt);

            isSolidColor = IsSolidColor(rt, rtWidth, rtHeight);

            RenderTexture.ReleaseTemporary(rt);

            return isSolidColor;
        }

        //泊松采样
        static bool IsSolidColor(RenderTexture rt, int rtWidth, int rtHeight)
        {
            int count = 0;
            Vector2 sample;
            int samplePointCount = rtWidth * rtHeight;
            int sampleCountLimit = samplePointCount;   //最大可采样数
            int iterLimit = 100; // 迭代上限, 100次内决定有效位置

            // RT像素画到一张小图上可以后面读取像素
            RenderTexture.active = rt;

            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();

            //把像素点读取存储到数组中
            List<Vector2> samplePoints = new List<Vector2>();
            for (int i = 0; i < rtWidth; i++)
            {
                for (int j = 0; j < rtHeight; j++)
                {
                    samplePoints.Add(new Vector2(i, j));
                }
            }

            Color initColor = new Color(0, 0, 0);
            while (samplePoints.Count > 0 && sampleCountLimit > 0 && iterLimit-- > 0)
            {
                int next = (int)Mathf.Lerp(0, samplePoints.Count - 1, UnityEngine.Random.value);
                sample = samplePoints[next]; // 在这些点中随便选一个采样点进行范围随机

                bool found = false;
                int kloop = 30; // 迭代30次, 找到泊松分布点
                float radius = 1;   //采样半径为1像素
                float radius2 = radius * radius;

                for (int j = 0; j < kloop; j++)
                {
                    var angle = 2 * Mathf.PI * UnityEngine.Random.value;
                    float r = Mathf.Sqrt(UnityEngine.Random.value * 3 * radius2 + radius2);
                    //得到临近分布点
                    Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    if (candidate.x <= rtWidth && candidate.y <= rtHeight && candidate.x >= 0 && candidate.y >= 0)
                    {
                        found = true;

                        samplePoints.Add(candidate);
                        radius2 = radius * radius;
                        count++;
                        //找到采样点 进行颜色采样
                        Color candidateColor = texture.GetPixelBilinear(candidate.x, candidate.y);
                        if (sampleCountLimit == samplePointCount)
                        {
                            initColor = candidateColor;
                        }
                        else
                        {
                            initColor += candidateColor;
                            if (candidateColor != initColor / count)
                            {
                                //如果颜色有差异，说明不是纯色纹理，直接返回false
                                return false;
                            }
                        }
                        samplePointCount--;
                        break;
                    }
                }

                if (!found)
                {
                    // 如果这个点找不到周围可用点则移出采样点列表
                    samplePoints[next] = samplePoints[samplePoints.Count - 1];
                    samplePoints.RemoveAt(samplePoints.Count - 1);
                }
            }
            return true;
        }

        public static void DrawGuiLine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}