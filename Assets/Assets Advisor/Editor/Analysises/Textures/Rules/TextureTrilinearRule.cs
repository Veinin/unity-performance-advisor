using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureTrilinearRule : AnalysistRule
    {
        public TextureTrilinearRule()
        {
            name = "过滤模式为Trilinear的纹理";
            desc = "Trilinear 三线性过滤（三线性插值），纹理会在不同的 mip 水平之间进行模糊，从而增加 GPU 开销。";
            thresholdValue = 10;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as TextureImporter).filterMode = FilterMode.Bilinear;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.obj as Texture2D).filterMode == FilterMode.Trilinear;
        }
    }
}