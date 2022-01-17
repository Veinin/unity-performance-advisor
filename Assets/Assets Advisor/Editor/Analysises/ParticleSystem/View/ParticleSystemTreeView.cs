using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Effect)]
    public class ParticleSystemTreeView : AnalysistTreeView
    {
        private enum ParticleSystemColumns
        {
            TextureSize = 3,
            TextureMemory = 4,
            ParticleCompCount = 5,
            ParticleCount = 6,
            DrawcallAvg = 7,
            DrawcallMax = 8,
            OverdrawAvg = 9,
            OverdrawMax = 10,

        }

        public ParticleSystemTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("引用纹理数量"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("引用纹理内存"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("组件数量"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("最大粒子数"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("平均 Drawcall"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 85,
                    minWidth = 85,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Drawcall 峰值"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 85,
                    minWidth = 85,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("平均 Overdraw"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 90,
                    minWidth = 90,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Overdraw 峰值"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 90,
                    minWidth = 90,
                },
            };
        }

        protected override void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var columnIndex = (ParticleSystemColumns) column;
            var columnValue = GetColumnValue(element, column);
            switch(columnIndex)
            {
                case ParticleSystemColumns.TextureMemory:
                    EditorGUI.LabelField(cellRect, AnalysistUtility.GetMemorySize(columnValue));
                    break;
                case ParticleSystemColumns.OverdrawAvg:
                case ParticleSystemColumns.OverdrawMax:
                    EditorGUI.LabelField(cellRect, columnValue.ToString("f2"));
                    break;
                default:
                    EditorGUI.LabelField(cellRect, columnValue.ToString());
                    break;
            }
        }

        protected override float GetColumnValue(AnalysistTreeElement element, int column)
        {
            var result = (element.item as ParticleSystemAssetItem).result;
            if (result == null)
            {
                return 0;
            }

            var summary = result.summary;
            var columnIndex = (ParticleSystemColumns) column;
            switch(columnIndex)
            {
                case ParticleSystemColumns.TextureSize:
                    return (float) summary.textureUseageMax;
                case ParticleSystemColumns.TextureMemory:
                    return (float) summary.memoryUsageMax;
                case ParticleSystemColumns.ParticleCompCount:
                    return (int) summary.particleCompMax;
                case ParticleSystemColumns.ParticleCount:
                    return (int) summary.particleCountMax;
                case ParticleSystemColumns.DrawcallAvg:
                    return (int) summary.drawCallAvg;
                case ParticleSystemColumns.DrawcallMax:
                    return (int) summary.drawCallMax;
                case ParticleSystemColumns.OverdrawAvg:
                    return (float) summary.overDrawAvg;
                case ParticleSystemColumns.OverdrawMax:
                    return (float) summary.overDrawMax;
            }
            return 0;
        }
    }
}