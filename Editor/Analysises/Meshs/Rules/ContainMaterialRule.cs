using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class ContainMaterialRule : AnalysistRule
    {
        public ContainMaterialRule()
        {
            name = "包含 Materials 的网格";
            desc = "如无必要建议不进行导入。";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return (item.importer as ModelImporter).materialImportMode != ModelImporterMaterialImportMode.None;
        }

        public override void OnBatching(AssetItem item)
        {
            var import = item.importer as ModelImporter;
            import.materialImportMode = ModelImporterMaterialImportMode.None;
        }
    }
}