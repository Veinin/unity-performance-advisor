namespace UPA
{
    [Rule(AssetType.Effect)]
    public class MaxTextureSizeRule : AnalysistRule
    {
        public MaxTextureSizeRule()
        {
            name = "特效贴图数量过大";
            desc = "统计每帧参与渲染的贴图内存，取最大值 > " + AssetAdvisorSettings.current.PSTextureLimit;
            thresholdValue = AssetAdvisorSettings.current.PSTextureLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item as ParticleSystemAssetItem).result.summary.textureUseageMax 
                > AssetAdvisorSettings.current.PSTextureLimit;
        }
    }
}