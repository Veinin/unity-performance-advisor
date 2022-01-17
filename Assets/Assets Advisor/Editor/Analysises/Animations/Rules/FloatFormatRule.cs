using UnityEngine;
using UnityEditor;

namespace UPA
{
    [Rule(AssetType.Animation)]
    public class FloatFormatRule : AnalysistRule
    {
        private const int LimitPrecision = 3;
        private static string ParseFormat = "f3";

        public FloatFormatRule()
        {
            name = "精度过高的动画片段";
            desc = "建议把精度缩减到 3～4 位，从而降低动画片段资源的内存占用。";
            thresholdValue = 10;
        }

        public override bool OnAnalysis(AssetItem item)
        {
            var clip = item.obj as AnimationClip;

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                var keys = curve.keys;
                foreach (var key in keys)
                {
                    if (IsFailFormat(key.value) || IsFailFormat(key.inTangent) || IsFailFormat(key.outTangent))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsFailFormat(float value)
        {
            return GetFloatLength(value) > LimitPrecision;
        }

        private int GetFloatLength(float value)
        {
            string s = value.ToString();
            return s.Length - s.IndexOf(".") - 1;
        }

        public override void OnBatching(AssetItem item)
        {
            var clip = item.obj as AnimationClip;

            foreach(var binding in AnimationUtility.GetCurveBindings(clip))
            {
                AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

                var keys = curve.keys;
                for (var i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    key.value = ParseLimitFormat(key.value);
                    key.inTangent = ParseLimitFormat(key.inTangent);
                    key.outTangent = ParseLimitFormat(key.outTangent);
                    keys[i] = key;
                }
                curve.keys = keys;

                clip.SetCurve(binding.path, binding.type, binding.propertyName, curve);
            }
        }

        private float ParseLimitFormat(float value)
        {
            return float.Parse(value.ToString(ParseFormat));
        }
    }
}