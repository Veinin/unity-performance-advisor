using System;
using System.Collections.Generic;
using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [Serializable]
    public class AnalysistTreeView : TreeViewWithTreeModel<AnalysistTreeElement>
    {
        private MultiColumnHeaderState m_headerState;

        enum Columns
        {
            Check,
            Name,
            Path,
        }

        public AnalysistTreeView(TreeViewState state, TreeModel<AnalysistTreeElement> model)
            : base(state, model)
        {
            InitColumnHader();
        }

        protected void InitColumnHader()
        {
            var columns = OnHeaderCreate();

            var fullColumns = new MultiColumnHeaderState.Column[3 + columns.Length];
            fullColumns[0] = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent(EditorGUIUtility.FindTexture("FilterByLabel")),
                contextMenuText = "Asset",
                headerTextAlignment = TextAlignment.Center,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Right,
                width = 30,
                minWidth = 30,
                maxWidth = 60,
            };
            fullColumns[1] = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 120,
                minWidth = 120,
                autoResize = true,
            };
            fullColumns[2] = new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Path"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Center,
                width = 300,
                minWidth = 300,
                autoResize = true,
            };

            for (var i = 3; i < fullColumns.Length; i++)
            {
                fullColumns[i] = columns[i - 3];
            }

            m_headerState = new MultiColumnHeaderState(fullColumns);

            multiColumnHeader = new MultiColumnHeader(m_headerState);
            multiColumnHeader.canSort = true;
            multiColumnHeader.sortingChanged += OnSortingChanged;
        }

        void OnSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            var sortIdx = multiColumnHeader.sortedColumnIndex;
            var ascend = multiColumnHeader.IsSortedAscending(sortIdx);
            
            var rows = m_TreeModel.GetData() as List<AnalysistTreeElement>;
            var root = rows[0];
            var children = rows.GetRange(1, rows.Count - 1);

            var ascendValue = ascend ? 1 : -1;
            Comparison<AnalysistTreeElement> comparison = (AnalysistTreeElement lhs, AnalysistTreeElement rhs) =>
            {
                var lv = GetColumnValue(lhs, sortIdx);
                var rv = GetColumnValue(rhs, sortIdx);
                return lv.CompareTo(rv) * ascendValue;
            };
            children.Sort(comparison);

            var elements = new List<AnalysistTreeElement>();
            elements.Add(new AnalysistTreeElement(-1, -1));
            elements.AddRange(children);

            Init(new TreeModel<AnalysistTreeElement>(elements));
            Reload();
        }

        protected virtual float GetColumnValue(AnalysistTreeElement element, int column)
        {
            return -1;
        }

        protected virtual MultiColumnHeaderState.Column[] OnHeaderCreate()
        {
            return new MultiColumnHeaderState.Column[0];
        }

        protected override void SingleClickedItem(int id)
        {
            var item = treeModel.Find(id).item;
            if (item == null)
            {
                return;
            }
            EditorGUIUtility.PingObject(item.obj);
            Selection.activeObject = item.obj;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<AnalysistTreeElement>)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        void CellGUI(Rect cellRect, TreeViewItem<AnalysistTreeElement> item, int column, ref RowGUIArgs args)
        {
            var data = item.data;

            var columenIndex = (Columns)column;
            switch (columenIndex)
            {
                case Columns.Check:
                    var toggleRect = cellRect;
                    toggleRect.x += (cellRect.width - 18) * 0.5f;
                    toggleRect.width = 18;
                    data.enabled = EditorGUI.Toggle(toggleRect, data.enabled);
                    break;
                case Columns.Name:
                    EditorGUI.LabelField(cellRect, data.item.obj.name);
                    break;
                case Columns.Path:
                    var path = data.item.path;
                    EditorGUI.LabelField(cellRect, path.Substring(7, path.Length - 7)); // Cut off 'Assets/'
                    break;
                default:
                    OnCellGUI(cellRect, data, column);
                    break;
            }
        }

        protected virtual void OnCellGUI(Rect cellRect, AnalysistTreeElement element, int column)
        {
            var columnValue = GetColumnValue(element, column);
            EditorGUI.LabelField(cellRect, columnValue.ToString());
        }

        public List<AssetItem> GetEnableElements()
        {
            List<AssetItem> result = new List<AssetItem>();

            var data = m_TreeModel.GetData();
            for (var i = 1; i < data.Count; i++)
            {
                var element = data[i];
                if (element.enabled)
                {
                    result.Add(element.item);
                }
            }

            return result;
        }
    }
}