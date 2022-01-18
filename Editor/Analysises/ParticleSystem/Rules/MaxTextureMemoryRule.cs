namespace UPA
{
    [Rule(AssetType.Effect)]
    public class MaxTextureMemoryRule : AnalysistRule
    {
        private const float MaxMemory = 1;

        public MaxTextureMemoryRule()
        {
            name = "特效贴图占用内存过高";
            desc = "统计每帧参与渲染的贴图内存，取最大值。";
            thresholdDesc = "内存 > " + MaxMemory + "MB";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item as ParticleSystemAssetItem).result.summary.memoryUsageMax > MaxMemory * 1024 * 1024;
        }
    }
}