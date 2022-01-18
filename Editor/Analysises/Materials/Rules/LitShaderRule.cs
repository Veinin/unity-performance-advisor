using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class LitShaderRule : AnalysistRule
    {
        public LitShaderRule()
        {
            name = "使用了 URP/Lit 的材质";
            desc = "使用了 Universal Render Pipeline/Lit 的材质球：可能生成非常多的变体，Lit 占用内存比较大，加载耗时会非常高，建议使用其他 Shader 代替";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var material = item.obj as Material;
            var shader = material.shader;
            return shader.name == "Universal Render Pipeline/Lit";
        }
    }
}