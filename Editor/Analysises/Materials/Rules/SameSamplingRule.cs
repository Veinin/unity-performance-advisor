using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class SampSamplingRule : AnalysistRule
    {
        public SampSamplingRule()
        {
            name = "包含相同纹理采样的材质";
            desc = "采样两张相同的纹理，会导致GPU性能的浪费。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var material = item.obj as Material;
            var shader = material.shader;

            Dictionary<string, bool> paths = new Dictionary<string, bool>();
            for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                    var texture = material.GetTexture(propertyName);
                    if (texture == null)
                    {
                        continue;
                    }

                    var texturePath = AssetDatabase .GetAssetPath(texture);
                    if (paths.ContainsKey(texturePath))
                    {
                        return true;
                    }
                    paths[texturePath] = true;
                }
            }

            return false;
        }
    }
}