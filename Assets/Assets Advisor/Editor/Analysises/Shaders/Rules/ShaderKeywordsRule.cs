using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Shader)]
    public class ShaderKeywordsRule : AnalysistRule
    {
        public ShaderKeywordsRule()
        {
            name = "全局关键字过多的 Shader";
            desc = "Unity 支持的全局关键字数量有限，建议根据实际情况将 global 类型的关键字改为 local 类型。";
            thresholdDesc = "全局关键字数 > " + AssetAdvisorSettings.current.ShaderKeywordsLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var shader = item.obj as Shader;
            return AnalysistUtility.GetShaderCheckResult(shader).KeywordCount 
                > AssetAdvisorSettings.current.ShaderKeywordsLimit;
        }
    }
}