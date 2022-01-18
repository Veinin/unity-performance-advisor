using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureReadWriteEnableRule : AnalysistRule
    {
        public TextureReadWriteEnableRule()
        {
            name = "开启Read/Write选项的纹理";
            desc = "Read/Write选项启用后，将会允许从脚本来访问纹理数据，所以在系统内存中会保留纹理数据的副本，占用额外内存，等同于一个纹理数据会有双倍的内存消耗。";
            thresholdValue = 10;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as TextureImporter).isReadable = false;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.importer as TextureImporter).isReadable;
        }
    }
}