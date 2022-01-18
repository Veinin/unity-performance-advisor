using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureAnisoLevelRule : AnalysistRule
    {
        public TextureAnisoLevelRule()
        {
            name = "开启各向异性过滤的纹理";
            desc = "对纹理进行各向异性过滤对于地面等物体的显示会有增益，但同时会带来较高的性能开销，建议排查。";
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as TextureImporter).anisoLevel = 1;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.obj as Texture2D).anisoLevel != 1;
        }
    }
}