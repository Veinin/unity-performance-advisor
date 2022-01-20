using System;
using System.Collections.Generic;
using UnityEngine;

namespace UPA
{
    [Serializable]
    [CreateAssetMenu(fileName = "ParticleSystemProfile", menuName = "Performance Advisor/Particle System Profile Asset")]
    public class ParticleSystemProfileAsset : ScriptableObject
    {
        public string profile = "";
        public string assetPath = "Assets/Res/VFX";
        public List<ParticleSystemResult> results;

        public Dictionary<string, ParticleSystemResult> ResultMapping
        {
            get {
                Dictionary<string, ParticleSystemResult> dic = new Dictionary<string, ParticleSystemResult>();
                foreach(var result in results)
                {
                    dic[result.path] = result;
                }
                return dic;
            }
        }

        public static ParticleSystemProfileAsset Load()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:ParticleSystemProfileAsset");
            if (guids.Length == 0)
            {
                Debug.LogWarning("Could not find ToolSettings asset. Will use default settings instead.");
                return ScriptableObject.CreateInstance<ParticleSystemProfileAsset>();
            }
            else
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<ParticleSystemProfileAsset>(path);
            }
        }
    }
}