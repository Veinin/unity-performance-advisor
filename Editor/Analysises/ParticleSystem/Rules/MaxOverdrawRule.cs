namespace UPA
{
    [Rule(AssetType.Effect)]
    public class MaxOverdrawRule : AnalysistRule
    {
        public MaxOverdrawRule()
        {
            name = "特效播放时最大 Overdraw 率过高";
            desc = "统计每帧参与渲染的像素的 Overdraw，并取过程中最高的值。该值越大，特效导致GPU压力的可能性也会越高，建议对其进行检查。";
            thresholdDesc = "最大Overdraw > " + AssetAdvisorSettings.current.PSMaxOverdrawLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item as ParticleSystemAssetItem).result.summary.drawCallMax 
                > AssetAdvisorSettings.current.PSMaxOverdrawLimit;
        }
    }
}