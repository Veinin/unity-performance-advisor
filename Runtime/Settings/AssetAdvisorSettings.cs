using System.Collections.Generic;
using UnityEngine;

namespace UPA
{
    [CreateAssetMenu(fileName = "New Profile Asset.asset", menuName = "Performance Advisor/Profile Asset")]
    public class AssetAdvisorSettings : ScriptableObject
    {
        [Tooltip("纹理最大分辨率大小")]
        public Vector2Int TextureResolutionLimit = new Vector2Int(512, 512);

        [Tooltip("Shader 全局关键字最大数量")]
        public int ShaderKeywordsLimit = 10;
        [Tooltip("Shader 纹理采样最大数")]
        public int ShaderTexEnvLimit = 5;
        [Tooltip("Shader 变体最大数量")]
        public int ShaderVariantLimit = 100;

        [Tooltip("Mesh 顶点数限制")]
        public int MeshVertsLimit = 3000;
        [Tooltip("Mesh 面片数限制")]
        public int MeshTrisLimit = 1500;

        [Tooltip("粒子平均 Overdraw 最大数")]
        public int PSAvgOverdrawLimit = 2;
        [Tooltip("粒子 Overdraw 最大数")]
        public int PSMaxOverdrawLimit = 2;
        [Tooltip("粒子 DrawCall 最大数")]
        public int PSDrawCallLimit = 2;
        [Tooltip("粒子最大贴图数量")]
        public int PSTextureLimit = 2;

        public static List<AssetAdvisorSettings> Load()
        {
            var settings = new List<AssetAdvisorSettings>();

            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AssetAdvisorSettings");
            if (guids.Length == 0)
            {
                Debug.LogWarning("Could not find ToolSettings asset. Will use default settings instead.");
                settings.Add(ScriptableObject.CreateInstance<AssetAdvisorSettings>());
            }
            else
            {
                foreach(var guid in guids)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetAdvisorSettings>(path);
                    settings.Add(asset);
                }
            }

            return settings;
        }

        private static List<AssetAdvisorSettings> m_SettingsList;
        public static List<AssetAdvisorSettings> settingList
        {
            get
            {
                if (m_SettingsList == null)
                {
                    m_SettingsList = AssetAdvisorSettings.Load();
                }
                return m_SettingsList;
            }
        }

        private static AssetAdvisorSettings m_Settings;
        public static AssetAdvisorSettings current
        {
            get 
            {
                if (m_Settings == null)
                {
                    m_Settings = settingList[0];
                }
                return m_Settings;
            }
            set
            {
                m_Settings = value;
            }
        }
    }
}