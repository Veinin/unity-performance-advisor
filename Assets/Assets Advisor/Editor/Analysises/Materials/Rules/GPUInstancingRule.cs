using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class GPUInstancingRule : AnalysistRule
    {
        public GPUInstancingRule()
        {
            name = "未开启 GPU Instancing 的材质";
            desc = "使用了支持 GPU Instancing 的材质，单未在材质中开启改选项。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var material = item.obj as Material;
            var shader = material.shader;

            var instancingOn = false;
            var keywrods = AnalysistUtility.GetShaderGlobalKeywords(shader);

            foreach(var k in keywrods)
            {
                if (k == "INSTANCING_ON")
                {
                    instancingOn = true;
                    break;
                }
            }

            if (!instancingOn) // 不支持 GPU Instancing
            {
                return false;
            }

            return !material.enableInstancing;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.obj as Material).enableInstancing = true;
        }
    }
}