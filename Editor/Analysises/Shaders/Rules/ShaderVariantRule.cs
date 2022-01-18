using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Shader)]
    public class ShaderVariantRule : AnalysistRule
    {
        public ShaderVariantRule()
        {
            name = "可能生成变体数过多的 Shader";
            desc = "预估变体数较多的 Shader，会占用较高内存和加载耗时，建议进行排查。";
            thresholdDesc = "预估变体值 >" + AssetAdvisorSettings.current.ShaderVariantLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var shader = item.obj as Shader;
            return AnalysistUtility.GetShaderCheckResult(shader).VariantCount 
                > AssetAdvisorSettings.current.ShaderVariantLimit;
        }
    }
}