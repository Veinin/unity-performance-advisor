using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureWrapModeRule : AnalysistRule
    {
        public TextureWrapModeRule()
        {
            name = "Wrap模式为Repeat的纹理";
            desc = "Wrapmode 使用了 Repeat 模式，容易导致贴图边缘出现杂色，建议进行检查。";
            thresholdValue = 10;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as TextureImporter).wrapMode = TextureWrapMode.Clamp;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.obj as Texture2D).wrapMode == TextureWrapMode.Repeat;
        }
    }
}