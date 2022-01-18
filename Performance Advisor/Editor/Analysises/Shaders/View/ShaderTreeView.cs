using System;
using System.Collections.Generic;
using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Shader)]
    public class ShaderTreeView : AnalysistTreeView
    {
        private enum ShaderColumns {
            TextureCount = 3,
            VariantCount = 4,
            LocalKeywords = 5,
            GlobalKeyWords = 6,
        }

        public ShaderTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Texture Count"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 100,
                    minWidth = 100,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Variant Count"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 90,
                    minWidth = 90,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Local Keyworlds"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 105,
                    minWidth = 105,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Global Keyworlds"),
                    headerTextAlignment = TextAlignment.Center,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 105,
                    minWidth = 105,
                },
            };
        }

        protected override void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var columnValue = GetColumnValue(element, column);
            EditorGUI.LabelField(cellRect, columnValue.ToString());
        }

        protected override float GetColumnValue(AnalysistTreeElement element, int index)
        {
            var shader = element.item.obj as Shader;
            var columeIndex = (ShaderColumns)index;
            var shaderCheckResult = AnalysistUtility.GetShaderCheckResult(shader);
            switch (columeIndex)
            {
                case ShaderColumns.TextureCount:
                    return shaderCheckResult.TexEnvCount;
                case ShaderColumns.VariantCount:
                    return shaderCheckResult.VariantCount;
                case ShaderColumns.LocalKeywords:
                    return shaderCheckResult.LocalKeywordCount;
                case ShaderColumns.GlobalKeyWords:
                    return shaderCheckResult.GlobakKeywordCount;
            }
            return -1;
        }

    }
}