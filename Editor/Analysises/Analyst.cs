using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using EditorKit;

namespace UPA
{
    /// <summary>
    /// 资源分析师
    /// </summary>
    [Serializable]
    public class Analyst
    {
        /// <summary>
        /// 分析资源类型
        /// </summary>
        public AssetType assetType;

        /// <summary>
        /// 分析对象
        /// </summary>
        public List<AssetItem> items = new List<AssetItem>();

        /// <summary>
        /// 分析资源规则
        /// </summary>
        /// <typeparam name="AnalysistRule"></typeparam>
        /// <returns></returns>
        public List<AnalysistRule> rules = new List<AnalysistRule>();

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold = true;
        /// <summary>
        /// 文件树
        /// </summary>
        private AnalysistTreeView m_TreeView;

        public AnalysistTreeView treeView
        {
            get
            {
                if (m_TreeView == null)
                {
                    m_TreeView = AnalystFactory.BuildTreeView(assetType, items);
                }
                return m_TreeView;
            }
            set
            {
                m_TreeView = value;
            }
        }

        public void Reset()
        {
            items.Clear();
            m_TreeView = null;
        }

        public void AddItem(AssetItem item)
        {
            items.Add(item);
        }

        public Analyst AddRules(AnalysistRule rule)
        {
            rules.Add(rule);
            return this;
        }

        public void ShowRuleGUI()
        {
            foreach (var rule in rules)
            {
                OnAnalystRuleGUI(rule);
            }
        }

        void OnAnalystRuleGUI(AnalysistRule rule)
        {
            GUILayout.BeginHorizontal();
            {
                var isPassed = rule.isPassed;
                GUILayout.Label(rule.name, GUILayout.Width(200));
                if (isPassed)
                {
                    GUILayout.Label("通过", GUILayout.Width(60));
                }
                else
                {
                    var style = new GUIStyle();
                    style.normal.textColor = new Color32(177, 12, 12, 255);
                    GUILayout.Label("失败*", style, GUILayout.Width(60));
                }
                GUILayout.Label(rule.failureItems.Count.ToString(), GUILayout.Width(80));
                GUILayout.Label(rule.threshold, GUILayout.Width(120));

                if (GUILayout.Button(rule.isFold ? ">" : "v", GUILayout.Width(50)))
                {
                    rule.isFold = !rule.isFold;
                }

                if (rule.failureItems.Count > 0 && rule.isBatching)
                {
                    if (GUILayout.Button("Fixed", GUILayout.Width(50)))
                    {
                        DoBatchingAssets(rule);
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (rule.isFold)
            {
                return;
            }

            EditorGUILayout.HelpBox(rule.desc, MessageType.Info);

            if (rule.itemCount > 0)
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                var treeRect = new Rect(8, lastRect.y + 40, lastRect.width, 150);
                rule.treeView.OnGUI(treeRect);
                GUILayout.Space(treeRect.height + 10);
            }
        }

        void DoBatchingAssets(AnalysistRule rule)
        {
            var cancel = false;
            var items = rule.items;
            var count = 1;
            var totalCount = items.Count;

            for (var i = items.Count - 1; i >= 0; i--, count++)
            {
                var item = items[i];

                cancel = EditorUtility.DisplayCancelableProgressBar("Batching Resources (" + count + "/" + totalCount + ")"
                    , item.path, count * 1f / totalCount);

                try
                {
                    rule.OnBatching(item);
                    rule.failureItems.Remove(item);
                    item.ReImport();
                }
                catch
                {
                    cancel = true;
                }

                if (cancel)
                {
                    break;
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }
    }
}