namespace UPA
{
    [Rule(AssetType.Effect)]
    public class MaxDrawCallRule : AnalysistRule
    {
        public MaxDrawCallRule()
        {
            name = "特效播放时 DrawCall 峰值过高";
            desc = "统计每帧的 DrawCall 数，并取过程中最高的值。该值较高时需要对其进行检查。";
            thresholdDesc = "DrawCall峰值 > " + AssetAdvisorSettings.current.PSDrawCallLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item as ParticleSystemAssetItem).result.summary.drawCallMax 
                > AssetAdvisorSettings.current.PSDrawCallLimit;
        }
    }
}