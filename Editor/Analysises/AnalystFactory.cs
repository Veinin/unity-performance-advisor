using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using EditorKit;

namespace UPA
{
    public static class AnalystFactory
    {
        public static Analyst BuildAnalyst(AssetType assetType)
        {
            Analyst analyst = new Analyst();
            analyst.assetType = assetType;

            List<Type> ruleTypes = GetRuleTypeDerivedFrom(assetType);
            foreach(var type in ruleTypes)
            {
                analyst.AddRules((AnalysistRule) Activator.CreateInstance(type));
            }

            return analyst;
        }

        public static List<Type> GetRuleTypeDerivedFrom(AssetType assetType)
        {
            var ruleTypes = new List<Type>();
            foreach (var type in RefrectionUtility.GetImplementationsOf(typeof(AnalysistRule)))
            {
                var attachAtt = type.GetCustomAttributes(typeof(RuleAttribute), true).FirstOrDefault() as RuleAttribute;
                if (attachAtt == null) {
                    continue;
                }

                if (attachAtt.assetType == assetType)
                {
                    ruleTypes.Add(type);
                }
            }
            return ruleTypes;
        }

        public static AnalysistTreeView BuildTreeView(AssetType assetType, List<AssetItem> assetItems)
        {
            var treeType = GetTreeViewType(assetType);
            var treeModel = GetTreeModel(assetItems);
            var treeState = new TreeViewState();

            object[] parameters = new object[2];
            parameters[0] = treeState;
            parameters[1] = treeModel;
            
            var treeView = Activator.CreateInstance(treeType, parameters) as AnalysistTreeView;
            treeView.Reload();
            return treeView;
        }

        private static Type GetTreeViewType(AssetType assetType)
        {
            foreach (var type in RefrectionUtility.GetImplementationsOf(typeof(AnalysistTreeView)))
            {
                var targetAtt = type.GetCustomAttributes(typeof(RuleAttribute), true).FirstOrDefault() as RuleAttribute;
                if (targetAtt == null) {
                    continue;
                }

                if (targetAtt.assetType == assetType)
                {
                    return type;
                }
            }
            return typeof(AnalysistTreeView);
        }

        private static TreeModel<AnalysistTreeElement> GetTreeModel(List<AssetItem> assetItems)
        {
            var elements = new List<AnalysistTreeElement>();
            elements.Add(new AnalysistTreeElement(-1, -1)); // Root
            for (var i = 0; i < assetItems.Count; i++)
            {
                elements.Add(new AnalysistTreeElement(i, 0, assetItems[i]));
            }
            return new TreeModel<AnalysistTreeElement>(elements);
        }
    }
}