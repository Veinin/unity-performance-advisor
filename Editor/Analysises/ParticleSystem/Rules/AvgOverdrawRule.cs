namespace UPA
{
    [Rule(AssetType.Effect)]
    public class AvgOverdrawRule : AnalysistRule
    {
        public AvgOverdrawRule()
        {
            name = "特效播放时平均 Overdraw 率过高";
            desc = "统计每帧参与渲染的像素的平均overdraw，并取过程中最高的值。该值越大，特效导致GPU压力的可能性也会越高，建议对其进行检查。";
            thresholdDesc = "平均Overdraw > " + AssetAdvisorSettings.current.PSAvgOverdrawLimit;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item as ParticleSystemAssetItem).result.summary.overDrawAvg 
                > AssetAdvisorSettings.current.PSAvgOverdrawLimit;
        }
    }
}