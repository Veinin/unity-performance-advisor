using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class MeshVertsRule : AnalysistRule
    {
        public MeshVertsRule()
        {
            name = "顶点数过大的网格";
            desc = "检测到顶点数 > " + AssetAdvisorSettings.current.MeshTrisLimit + "，建议检测其必要性。(顶点数限制数值可以更改配置文件)";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return AnalysistUtility.GetMeshVerts(item.obj as GameObject) > AssetAdvisorSettings.current.MeshTrisLimit;
        }
    }
}