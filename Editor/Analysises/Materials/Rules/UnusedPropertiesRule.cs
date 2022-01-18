using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class UnusedPropertiesRule : AnalysistRule
    {
        public UnusedPropertiesRule()
        {
            name = "包含未使用属性的材质";
            desc = @"即使更换 Shader 也不会把原来依赖的属性去除。所以可能会造成不需要的属性进包体内，从而造成内存的浪费。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var material = item.obj as Material;
            var so = new SerializedObject(material);
            so.Update();

            var savedProperties = so.FindProperty("m_SavedProperties");

            if (CheckUnusedProperties(material, savedProperties.FindPropertyRelative("m_Floats")))
            {
                return true;
            }

            if (CheckUnusedProperties(material, savedProperties.FindPropertyRelative("m_Colors")))
            {
                return true;
            }

            return false;
        }

        private bool CheckUnusedProperties(Material material, SerializedProperty sp)
        {
            for (int i = sp.arraySize - 1; i >= 0; i--)
            {
                var property = sp.GetArrayElementAtIndex(i).FindPropertyRelative("first");
                if (!material.HasProperty(property.stringValue))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnBatching(AssetItem item)
        {
            var material = item.obj as Material;
            var so = new SerializedObject(material);
            var savedProperties = so.FindProperty("m_SavedProperties");

            RemoveUnusedProperties(material, savedProperties.FindPropertyRelative("m_Floats"));
            RemoveUnusedProperties(material, savedProperties.FindPropertyRelative("m_Colors"));

            so.ApplyModifiedProperties();
        }

        private void RemoveUnusedProperties(Material material, SerializedProperty sp)
        {
             for (int i = sp.arraySize - 1; i >= 0; i--)
            {
                var property = sp.GetArrayElementAtIndex(i).FindPropertyRelative("first");
                if (!material.HasProperty(property.stringValue))
                {
                    sp.DeleteArrayElementAtIndex(i);
                }
            }
        }
    }
}