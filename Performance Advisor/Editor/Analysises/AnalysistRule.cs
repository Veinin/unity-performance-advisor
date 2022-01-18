using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace UPA
{
    /// <summary>
    /// 资源分析规则
    /// </summary>
    [Serializable]
    public class AnalysistRule
    {
        /// <summary>
        /// 检测规则名称
        /// </summary>
        public string name;
        /// <summary>
        /// 检测规则描述
        /// </summary>
        public string desc;
        /// <summary>
        /// 检测失败阈值
        /// </summary>
        public int thresholdValue;
        /// <summary>
        /// 检测失败阈值描述
        /// </summary>
        public string thresholdDesc;
        /// <summary>
        /// 失败对象
        /// </summary>
        /// <typeparam name="Object"></typeparam>
        /// <returns></returns>
        public List<AssetItem> failureItems = new List<AssetItem>();

        /// <summary>
        /// 是否折叠
        /// </summary>
        public bool isFold = true;
        /// <summary>
        /// 文件树
        /// </summary>
        private AnalysistTreeView m_TreeView;

        public string threshold
        {
            get
            {
                if (string.IsNullOrEmpty(thresholdDesc))
                {
                    return "总数>" + thresholdValue;
                }
                return thresholdDesc;
            }
        }

        public bool isPassed
        {
            get
            {
                return failureItems.Count <= thresholdValue;
            }
        }

        public bool isBatching
        {
            get
            {
                var method = this.GetType().GetMethod("OnBatching", BindingFlags.Instance | BindingFlags.Public);
                return method != null && !method.DeclaringType.Equals(typeof(AnalysistRule));
            }
        }

        public int itemCount
        {
            get
            {
                return failureItems.Count;
            }
        }

        public List<AssetItem> items
        {
            get
            {
                return m_TreeView != null ? m_TreeView.GetEnableElements() : failureItems;
            }
        }

        public AssetType assetType
        {
            get
            {
                var attr = GetType().GetCustomAttributes(typeof(RuleAttribute), true).FirstOrDefault() as RuleAttribute;
                return attr.assetType;
            }
        }

        public AnalysistTreeView treeView
        {
            get
            {
                if (m_TreeView == null)
                {
                    m_TreeView = AnalystFactory.BuildTreeView(assetType, failureItems);
                }
                return m_TreeView;
            }
        }

        /// <summary>
        /// 分析资源
        /// </summary>
        /// <param name="item"></param>
        public void Analyze(AssetItem item)
        {
            var hit = OnAnalysis(item);
            if (hit)
            {
                failureItems.Add(item);
            }
        }

        public virtual bool OnAnalysis(AssetItem item)
        {
            return false;
        }

        public virtual void OnBatching(AssetItem item)
        {

        }
    }
}