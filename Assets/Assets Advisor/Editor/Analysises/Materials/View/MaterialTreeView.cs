using System;
using System.Collections.Generic;
using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Material)]
    public class MaterialTreeView : AnalysistTreeView
    {
        private const int ColumnShader = 3;

        public MaterialTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Shader"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 300,
                    minWidth = 200,
                },
            };
        }

        protected override void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var material = element.item.obj as Material;
            if (column == ColumnShader)
            {
                EditorGUI.LabelField(cellRect, material.shader.name);
            }
        }

    }
}