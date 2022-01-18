using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureSizeRule : AnalysistRule
    {
        public TextureSizeRule()
        {
            var size = AssetAdvisorSettings.current.TextureResolutionLimit.x + "*" 
                + AssetAdvisorSettings.current.TextureResolutionLimit.y;

            name = "尺寸过大的纹理";
            desc = "一般来说，纹理尺寸越大，占用的内存也就越大，一般情况我们推荐纹理尺寸为 " + size + "，如果此分辨率显示效果已经够用，" 
                + "那么就不要用 1024*1024 的纹理，因为后者的内存占用是前者的 4 倍。";
            thresholdDesc = "长宽>" + size;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var texture = item.obj as Texture2D;
            return texture.width > AssetAdvisorSettings.current.TextureResolutionLimit.x 
                || texture.height > AssetAdvisorSettings.current.TextureResolutionLimit.y;
        }
    }
}