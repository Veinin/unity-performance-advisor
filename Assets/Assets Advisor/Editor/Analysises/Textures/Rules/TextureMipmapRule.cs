using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureMipmapRule : AnalysistRule
    {
        public TextureMipmapRule()
        {
            name = "开启Mipmap选项的Sprite纹理";
            desc = @"Mipmap开启后，内存会是未开启Mipmap的 1.33 倍，因为Mipmap会生成一组长宽依次减少一倍的纹理序列，一直生成到 1*1。"
                + "Mipmap 提升 GPU 效率，一般用于 3D 场景或角色，UI 不建议开启。";
            thresholdValue = 5;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as TextureImporter).mipmapEnabled = false;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.obj as Texture2D).mipmapCount > 1;
        }
    }
}