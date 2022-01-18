using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class MeshTrisRule : AnalysistRule
    {
        public MeshTrisRule()
        {
            name = "面片数过大的网格";
            desc = "检测到面片 > " + AssetAdvisorSettings.current.MeshTrisLimit 
                + "，建议检测其必要性。(面数限制数值可以更改配置文件)";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            return AnalysistUtility.GetMeshTris(item.obj as GameObject) 
                > AssetAdvisorSettings.current.MeshTrisLimit;
        }
    }
}