using System.IO;
using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Animation)]
    public class AnimationTreeView : AnalysistTreeView
    {
        private enum AnimationColumns
        {
            Length = 3,
            FileSize = 4,
            MemorySize = 5,
            CompressType = 6,
        }

        public AnimationTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Length"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 60,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("File Size"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 60,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Memory Size"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                }
            };
        }

        protected override void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var clip = element.item.obj as AnimationClip;
            var columeIndex = (AnimationColumns)column;
            var columnValue = GetColumnValue(element, column);
            switch(columeIndex)
            {
                case AnimationColumns.Length:
                    EditorGUI.LabelField(cellRect, columnValue.ToString());
                    break;
                case AnimationColumns.FileSize:
                case AnimationColumns.MemorySize:
                    EditorGUI.LabelField(cellRect, AnalysistUtility.GetMemorySize(columnValue));
                    break;
            }
        }

        protected override float GetColumnValue(AnalysistTreeElement element, int column)
        {
            var clip = element.item.obj as AnimationClip;
            var columeIndex = (AnimationColumns)column;
            switch(columeIndex)
            {
                case AnimationColumns.Length:
                    return clip.length;
                case AnimationColumns.FileSize:
                    return new FileInfo(element.item.path).Length;
                case AnimationColumns.MemorySize:
                    return Profiler.GetRuntimeMemorySizeLong(clip);
            }
            return 0;
        }
    }
}