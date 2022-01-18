using UnityEngine;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class MeshScaleRule : AnalysistRule
    {
        public MeshScaleRule()
        {
            name = "模型导出比例标尺不统一";
            desc = "模型的比例标尺与 Unity 大小必须 1:1";
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var go = item.obj as GameObject;

            var transform = go.GetComponent<Transform>();
            if (transform.localScale != Vector3.one)
            {
                return true;
            }

            var transforms = go.GetComponentsInChildren<Transform>();
            foreach(var t in transforms)
            {
                if (t.localScale != Vector3.one)
                {
                    return true;
                }
            }

            return false;
        }
    }
}