using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class LoseTextureSamplingRule : AnalysistRule
    {
        public LoseTextureSamplingRule()
        {
            name = "包含无用纹理采样的材质";
            desc = @"由于 Unity 的机制，材质球会自动保存其上的纹理采样，即使更换 Shader 也不会把原来依赖的纹理去除。
                所以可能会造成误依赖实际不需要的纹理带进包体的情况。从而造成内存的浪费。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return HasLostTextures(item.obj as Material);
        }

        public override void OnBatching(AssetItem item)
        {
            CleanMaterialSerializedProperty(item.obj as Material);
        }

        private bool HasLostTextures(Material material)
        {
            SerializedProperty texEnvs = GetTexEnvs(material);
            for (var i = 0; i < texEnvs.arraySize - 1; i++)
            {
                var element = texEnvs.GetArrayElementAtIndex(i);
                var elementName = element.FindPropertyRelative("first").stringValue;
                if (!material.HasProperty(elementName))
                {
                    return true;
                }
            }
            return false;
        }

        private SerializedProperty GetTexEnvs(Material material)
        {
            SerializedObject so = new SerializedObject(material);
            return so.FindProperty("m_SavedProperties").FindPropertyRelative("m_TexEnvs");
        }

        private void CleanMaterialSerializedProperty(Material material)
        {
            SerializedObject so = new SerializedObject(material);
            SerializedProperty texEnvs = so.FindProperty("m_SavedProperties").FindPropertyRelative("m_TexEnvs");

            for (var i = texEnvs.arraySize - 1; i >= 0; i--)
            {
                var element = texEnvs.GetArrayElementAtIndex(i);
                var elementName = element.FindPropertyRelative("first").stringValue;
                if (!material.HasProperty(elementName))
                {
                    //_MainTex是内建属性，是置空不删除，否则UITexture等控件在获取mat.maintexture的时候会报错
                    if (elementName.Equals("_MainTex"))
                    {
                        var texture = element.FindPropertyRelative("second").FindPropertyRelative("m_Texture");
                        if (texture.objectReferenceValue != null)
                        {
                            texture.objectReferenceValue = null;
                        }
                    }
                    else
                    {
                        texEnvs.DeleteArrayElementAtIndex(i);
                    }
                }
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(material);
        }
    }
}