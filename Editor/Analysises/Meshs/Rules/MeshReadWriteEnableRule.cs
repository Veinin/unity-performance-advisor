using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class MeshReadWriteEnableRule : AnalysistRule
    {
        public MeshReadWriteEnableRule()
        {
            name = "开启Read/Write选项的网格";
            desc = "Read/Write 选项启用后，将会允许从脚本来访问网格数据，同时会产生网格数据的副本，占用额外内存，等同于一个网格数据会有接近2倍的内存消耗。" +
                "对于需要使用StaticBatchingUtility.Combine进行合批的Mesh，以及部分Unity版本中粒子系统里使用到的Mesh，仍需要开启Mesh的Read/Write选项。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.importer as ModelImporter).isReadable;
        }

        public override void OnBatching(AssetItem item)
        {
            (item.importer as ModelImporter).isReadable = false;
        }
    }
}