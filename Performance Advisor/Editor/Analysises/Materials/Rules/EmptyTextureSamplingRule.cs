using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class EmptyTextureSamplingRule : AnalysistRule
    {
        public EmptyTextureSamplingRule()
        {
            name = "包含空纹理采样的材质";
            desc = "去掉空采样可以减少 GPU 的压力。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var material = item.obj as Material;
            var shader = material.shader;

            for (var i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    var propertyName = ShaderUtil.GetPropertyName(shader, i);
                    if (material.GetTexture(propertyName) == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}