using System.Collections.Generic;
using EditorKit;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Rule(AssetType.Mesh)]
    public class MeshTreeView : AnalysistTreeView
    {
        private enum MeshColumns {
            Verts   = 3,
            Tris    = 4,
        }

        public MeshTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
        }

        protected override MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Tris"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 100,
                    minWidth = 100,
                },

                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Verts"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Center,
                    width = 100,
                    minWidth = 100,
                },
            };
        }

        private Dictionary<int, int> tris = new Dictionary<int, int>();
        private Dictionary<int, int> verts = new Dictionary<int, int>();

        protected override float GetColumnValue(AnalysistTreeElement element, int index)
        {
            var go = element.item.obj as GameObject;
            var id = go.GetInstanceID();

            var columeIndex = (MeshColumns)index;
            switch (columeIndex)
            {
                case MeshColumns.Tris:
                    return AnalysistUtility.GetMeshTris(go);
                case MeshColumns.Verts:
                    return AnalysistUtility.GetMeshVerts(go);
            }
            return -1;
        }

    }
}