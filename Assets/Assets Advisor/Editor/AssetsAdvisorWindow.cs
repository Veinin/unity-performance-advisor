using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace UPA
{
    public enum AssetType
    {
        All = 0,
        Texture = 1,
        Mesh = 2,
        Material = 3,
        Shader = 4,
        Animation = 5,
        Prefab = 6,
        Effect = 7,
    }

    [Serializable]
    public class AssetItem
    {
        private Object m_Obj;
        private string m_Path;
        private AssetImporter m_Importer;

        public AssetItem(Object obj, string path)
        {
            m_Obj = obj;
            m_Path = path;
        }

        public Object obj { get { return m_Obj; } }
        public string path { get { return m_Path; } }

        public AssetImporter importer
        {
            get
            {
                if (m_Importer == null)
                {
                    m_Importer = AssetImporter.GetAtPath(m_Path);
                }
                return m_Importer;
            }
        }

        public void ReImport()
        {
            AssetDatabase.ImportAsset(m_Path);
        }
    }

    [Serializable]
    public class ProgressGUI
    {
        public int count = 0;
        public int total = 0;
        public bool isCancelable = true;
        public bool cancel = false;

        public void Show(string title, string info = "")
        {
            var progress = ++count * 1f / total;
            if (isCancelable)
            {
                cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
            }
            else
            {
                EditorUtility.DisplayProgressBar(title, info, progress);
            }
        }

        public void Clear()
        {
            cancel = true;
            EditorUtility.ClearProgressBar();
        }

        public void Reset(int t)
        {
            count = 0;
            total = t;
        }
    }

    public class AssetsAdvisorWindow : EditorWindow
    {
        /// <summary>
        /// 资源过滤器
        /// </summary>
        /// <value></value>
        public static string[] Filters = new string[] { "", "t:texture2D", "t:mesh", "t:material", "t:shader", "t:animation", "t:prefab" };
        /// <summary>
        /// 资源后缀名对应类型
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="AssetType"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, AssetType> SuffixAssetTypeMapping = new Dictionary<string, AssetType>()
        {
            {".png", AssetType.Texture},
            {".jpg", AssetType.Texture},
            {".fbx", AssetType.Mesh},
            {".mat", AssetType.Material},
            {".shader", AssetType.Shader},
            {".anim", AssetType.Animation},
            {".prefab", AssetType.Prefab},
        };

        [MenuItem("Tools/Performance Advisor/Assets Advisor")]
        public static void Init()
        {
            AssetsAdvisorWindow window = (AssetsAdvisorWindow)EditorWindow.GetWindow(typeof(AssetsAdvisorWindow));
            window.titleContent = new GUIContent("Assets Advisor");
            window.minSize = new Vector2(680, 250);
        }

        private AssetType m_Type;
        private DefaultAsset m_Folder;
        private bool m_Preview;
        private string m_FilteringText;
        private Dictionary<AssetType, Analyst> m_analysts = new Dictionary<AssetType, Analyst>();

        void OnGUI()
        {
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Assets Advisor");

            OnSettingsGUI();
            OnPreviewGUI();
            OnAnalysisGUI();
            OnAnalystResultGUI();
        }

        void OnSettingsGUI()
        {
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUI.skin.label.fontSize = 12;
            GUILayout.Label("Settings");
            GUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Asset Type:", GUILayout.Width(80));
                    var type = (AssetType)EditorGUILayout.EnumPopup(m_Type, GUILayout.Width(100));
                    if (type != AssetType.Effect)
                    {
                        m_Type = type;
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Asset Path:", GUILayout.Width(80));
                    m_Folder = (DefaultAsset)EditorGUILayout.ObjectField(m_Folder, typeof(DefaultAsset), false);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Asset Profile:", GUILayout.Width(80));

                    var settingList = AssetAdvisorSettings.settingList;
                    var settingNames = new string[settingList.Count];
                    var settingIndex = 0;

                    for (var i = 0; i < settingList.Count; i++)
                    {
                        var settings = settingList[i];
                        settingNames[i] = settings.name;
                        if (settings == AssetAdvisorSettings.current)
                        {
                            settingIndex = i;
                        }
                    }

                    var index = EditorGUILayout.Popup(settingIndex, settingNames);
                    if (index != settingIndex)
                    {
                        AssetAdvisorSettings.current = settingList[index];
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Asset Filter:", GUILayout.Width(80));
                    m_FilteringText = EditorGUILayout.TextField(m_FilteringText);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Preview:", GUILayout.Width(80));
                    m_Preview = EditorGUILayout.Toggle(m_Preview);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        void OnPreviewGUI()
        {
            if (!m_Preview || m_analysts.Count == 0)
            {
                return;
            }

            GUILayout.Space(10);

            GUILayout.Label("Preview");
            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("资源类型", GUILayout.Width(100));
                    GUILayout.Label("资源个数", GUILayout.Width(100));
                    GUILayout.Label("操作", GUILayout.Width(100));
                }
                GUILayout.EndHorizontal();

                AnalysistUtility.DrawGuiLine();

                foreach (var analyst in m_analysts.Values)
                {
                    if (m_Type != AssetType.All && m_Type != analyst.assetType)
                    {
                        continue;
                    }
                    OnPreviewAnalystGUI(analyst);
                }
            }
            GUILayout.EndVertical();
        }

        void OnPreviewAnalystGUI(Analyst analyst)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(analyst.assetType.ToString(), GUILayout.Width(100));
                GUILayout.Label(analyst.items.Count.ToString(), GUILayout.Width(100));
                if (GUILayout.Button(analyst.isFold ? ">" : "v", GUILayout.Width(50)))
                {
                    analyst.isFold = !analyst.isFold;
                }
            }
            GUILayout.EndHorizontal();

            if (analyst.isFold)
            {
                return;
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            var treeRect = new Rect(8, lastRect.y + 20, lastRect.width, 150);
            analyst.treeView.OnGUI(treeRect);
            GUILayout.Space(treeRect.height);
        }

        void OnAnalysisGUI()
        {
            if (m_Folder == null)
            {
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Analysis");

            GUILayout.BeginVertical("Box", GUILayout.Width(100));
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Start", GUILayout.Height(25), GUILayout.Width(50)))
                    {
                        DoScanningAssets();
                        return;
                    }
                    if (GUILayout.Button("Clear", GUILayout.Height(25), GUILayout.Width(50)))
                    {
                        m_analysts.Clear();
                        return;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        void OnAnalystResultGUI()
        {
            if (m_analysts.Count <= 0)
            {
                return;
            }

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("检测规则", GUILayout.Width(200));
                GUILayout.Label("检测结果", GUILayout.Width(60));
                GUILayout.Label("失败个数", GUILayout.Width(80));
                GUILayout.Label("阈值", GUILayout.Width(120));
                GUILayout.Label("操作", GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();

            AnalysistUtility.DrawGuiLine();

            foreach (var analyst in m_analysts.Values)
            {
                if (m_Type != AssetType.All && m_Type != analyst.assetType)
                {
                    continue;
                }

                analyst.ShowRuleGUI();
            }

            GUILayout.EndVertical();
        }

        void DoScanningAssets()
        {
            CollectionResources();
            AnalysisReousrces();
        }

        void CollectionResources()
        {
            m_analysts.Clear();
            AssetDatabase.Refresh();

            var cancel = false;
            var assetFolderPath = AssetDatabase.GetAssetPath(m_Folder);
            var assetGuids = AssetDatabase.FindAssets(Filters[(int) m_Type], new string[] { assetFolderPath });

            var idx = 0;
            var guids = new HashSet<string>();
            
            foreach(var guid in assetGuids)
            {
                guids.Add(guid);
            }
            
            foreach(var guid in guids)
            {
                cancel = EditorUtility.DisplayCancelableProgressBar("Collect Resources", idx + 1 + "/" + guids.Count, idx++ * 1f / guids.Count);

                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.Contains("."))
                {
                    continue;
                }

                var assetType = GetAssetTypeByPath(assetPath);
                if (assetType == AssetType.All)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(m_FilteringText) && !assetPath.Contains(m_FilteringText))
                {
                    continue;
                }

                var assetObj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                AddAssetItem(assetType, assetPath, assetObj);

                if (cancel)
                {
                    break;
                }
            }
        }

        AssetType GetAssetTypeByPath(string assetPath)
        {
            var suffixIndex = assetPath.LastIndexOf(".");
            var suffix = assetPath.Substring(suffixIndex, assetPath.Length - suffixIndex);
            if (SuffixAssetTypeMapping.ContainsKey(suffix))
            {
                return SuffixAssetTypeMapping[suffix];
            }
            else
            {
                return AssetType.All;
            }
        }

        void AddAssetItem(AssetType assetType, string assetPath, Object assetObj)
        {
            if (!m_analysts.ContainsKey(assetType))
            {
                m_analysts[assetType] = AnalystFactory.BuildAnalyst(assetType);
            }
            m_analysts[assetType].AddItem(new AssetItem(assetObj, assetPath));
        }

        void AnalysisReousrces()
        {
            var progress = new ProgressGUI();

            foreach (var analyst in m_analysts.Values)
            {
                progress.total += analyst.rules.Count;
            }

            foreach (var analyst in m_analysts.Values)
            {
                AnalysisUniversalAssets(analyst, progress);

                if (progress.cancel)
                {
                    break;
                }
            }

            progress.Clear();
        }

        void AnalysisUniversalAssets(Analyst analyst, ProgressGUI progress)
        {
            foreach (var rule in analyst.rules)
            {
                progress.Show("Analysis Resources", rule.name);

                try
                {
                    foreach (var item in analyst.items)
                    {
                        rule.Analyze(item);
                    }
                }
                catch
                {
                    progress.cancel = true;
                    break;
                }

                if (progress.cancel)
                {
                    break;
                }
            }
        }
    }
}