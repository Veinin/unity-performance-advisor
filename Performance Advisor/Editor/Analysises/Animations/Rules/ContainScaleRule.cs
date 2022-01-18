using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Animation)]
    public class ContainScaleRule : AnalysistRule
    {
        private static string ScaleKeyWord = "scale";

        public ContainScaleRule()
        {
            name = "包含Scale曲线的动画片段";
            desc = "Scale 缩放动画，一般角色上不会使用，将其移除可以减少一些内存。";
            thresholdValue = 10;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var clip = item.obj as AnimationClip;
            foreach (var curveBinding in AnimationUtility.GetCurveBindings(clip))
            {
                string name = curveBinding.propertyName.ToLower();
                if (name.Contains(ScaleKeyWord))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnBatching(AssetItem item)
        {
            var clip = item.obj as AnimationClip;
            foreach (var curveBinding in AnimationUtility.GetCurveBindings(clip))
            {
                string name = curveBinding.propertyName.ToLower();
                if (name.Contains(ScaleKeyWord))
                {
                    AnimationUtility.SetEditorCurve(clip, curveBinding, null);
                }
            }
        }
    }
}