using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace EditorKit
{
	internal class MultiColumnTreeView : TreeViewWithTreeModel<ParticleSystemElement>
	{
		const float kRowHeights = 20f;
		const float kToggleWidth = 18f;

		// All columns
		enum MyColumns
		{
			Check,
			Id,
			Name,
			Duration
		}


		public MultiColumnTreeView (TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<ParticleSystemElement> model) : base (state, multiColumnHeader, model)
		{
			// Custom setup
			rowHeight = kRowHeights;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			columnIndexForTreeFoldouts = 2;
			customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
			extraSpaceBeforeIconAndLabel = kToggleWidth;
			Reload();
		}


		// Note we We only build the visible rows, only the backend has the full tree information. 
		// The treeview only creates info for the row list.
		protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
		{
			var rows = base.BuildRows (root);
			return rows;
		}

		protected override void SingleClickedItem(int id)
		{
			var itemPath = treeModel.Find(id).path;
			Object o = AssetDatabase.LoadAssetAtPath<Object>(itemPath);
			EditorGUIUtility.PingObject(o);
			Selection.activeObject = o;
			Debug.Log(itemPath);
		}
		
		

		protected override void RowGUI (RowGUIArgs args)
		{
			var item = (TreeViewItem<ParticleSystemElement>) args.item;

			for (int i = 0; i < args.GetNumVisibleColumns (); ++i)
			{
				CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
			}
		}

		void CellGUI (Rect cellRect, TreeViewItem<ParticleSystemElement> item, MyColumns column, ref RowGUIArgs args)
		{
			// Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
			CenterRectUsingSingleLineHeight(ref cellRect);

			switch (column)
			{
				case MyColumns.Check:
					{
						var toggleRect = cellRect;
						toggleRect.x += (cellRect.width - kToggleWidth) * 0.5f;
						toggleRect.width = kToggleWidth;
						var oldVal = item.data.enabled;
						item.data.enabled = EditorGUI.Toggle(toggleRect, item.data.enabled);
						if (item.data.enabled != oldVal && GetSelection().Count > 1)
						{
							foreach (var idx in GetSelection())
							{
								treeModel.GetData()[idx].enabled = item.data.enabled;
							}
						}
					}
					break;
				case MyColumns.Id:
				{
					cellRect.xMin += 3f;
					EditorGUI.LabelField(cellRect, item.data.index.ToString());
				}
					break;
				case MyColumns.Name:
					{
						// Default icon and label
						cellRect.xMin += 3f;
						var iconRect = cellRect;
						iconRect.width = kRowHeights;
						GUI.DrawTexture(iconRect, item.data.icon, ScaleMode.ScaleToFit);
						cellRect.xMin += kRowHeights;
						EditorGUI.LabelField(cellRect, item.data.displayName);

					}
					break;

				case MyColumns.Duration:
				{
					cellRect.xMin += 3f;
					DefaultGUI.Label(cellRect, item.data.duration.ToString("f2"), args.selected, args.focused);
				}
				break;
			}
		}
	}
	
}
