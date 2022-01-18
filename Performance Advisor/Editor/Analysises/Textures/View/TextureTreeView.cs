using System;
using System.Collections.Generic;
using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Texture)]
    public class TextureTreeView : AnalysistTreeView
    {
        private enum TextureColumns
        {
            Memory = 3,
            Width = 4,
            Height = 5,
            Mipmap = 6,
        }

        public TextureTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Memory"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 60,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Width"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 60,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Height"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 60,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Mipmap Count"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 80,
                    minWidth = 80,
                }
            };
        }

        protected override void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var columnValue = GetColumnValue(element, column);
            if (column == (int) TextureColumns.Memory)
            {
                DrawMemoryLabel(cellRect, columnValue);
            }
            else
            {
                EditorGUI.LabelField(cellRect, columnValue.ToString());
            }
        }

        private void DrawMemoryLabel(Rect cellRect, float value)
        {
            EditorGUI.LabelField(cellRect, AnalysistUtility.GetMemorySize(value));
        }

        protected override float GetColumnValue(AnalysistTreeElement element, int index)
        {
            var texture = element.item.obj as Texture2D;
            var columeIndex = (TextureColumns)index;
            switch (columeIndex)
            {
                case TextureColumns.Memory:
                    return Profiler.GetRuntimeMemorySizeLong(texture);
                case TextureColumns.Width:
                    return texture.width;
                case TextureColumns.Height:
                    return texture.height;
                case TextureColumns.Mipmap:
                    return texture.mipmapCount;
            }
            return -1;
        }
    }
}