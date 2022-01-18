using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Shader)]
    public class ShaderTexEnvRule : AnalysistRule
    {
        public ShaderTexEnvRule()
        {
            name = "纹理采样数过多的 Shader";
            desc = "纹理数量过多，可能导致 GPU 不必要的开销。";
            thresholdDesc = "纹理采样数 > " + AssetAdvisorSettings.current.ShaderTexEnvLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var shader = item.obj as Shader;
            return AnalysistUtility.GetShaderCheckResult(shader).TexEnvCount 
                > AssetAdvisorSettings.current.ShaderTexEnvLimit;
        }
    }
}