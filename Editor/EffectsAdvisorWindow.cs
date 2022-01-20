using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.EditorCoroutines.Editor;
using Object = UnityEngine.Object;

namespace UPA
{
    [Serializable]
    public class ParticleSystemAssetItem : AssetItem
    {
        private ParticleSystemResult m_Rresult;

        public ParticleSystemAssetItem(Object obj, string path) : base(obj, path)
        {
            m_Rresult = new ParticleSystemResult();
        }

        public ParticleSystemResult result
        {
            set { m_Rresult = value; }
            get { return m_Rresult; }
        }
    }

    public class EffectsAdvisorWindow : EditorWindow
    {
        [MenuItem("Tools/Performance Advisor/Effects Advisor")]
        public static void Init()
        {
            EffectsAdvisorWindow window = (EffectsAdvisorWindow)EditorWindow.GetWindow(typeof(EffectsAdvisorWindow));
            window.titleContent = new GUIContent("Effects Advisor");
            window.minSize = new Vector2(680, 250);
        }

        private bool m_IsPlaying;
        private int m_ProfileIndex;
        private ProgressGUI m_ParticleSystemProgress;
        private ParticleSystemProfileAsset m_ProfileAsset;
        private DefaultAsset m_Folder;
        private Analyst m_Analyst;

        private void OnEnable()
        {
            InitAssets();
            InitAnalystItemRules();

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void InitAssets()
        {
            m_ProfileAsset = ParticleSystemProfileAsset.Load();
            m_Folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_ProfileAsset.assetPath);
            AssetAdvisorSettings.current = AssetDatabase.LoadAssetAtPath<AssetAdvisorSettings>(m_ProfileAsset.profile);
        }

        private void InitAnalystItemRules()
        {
            InitAnalyst();
            InitParticleSystemItems();
            InitRules();
        }

        private void InitAnalyst()
        {
            m_Analyst = AnalystFactory.BuildAnalyst(AssetType.Effect);
        }

        private void InitParticleSystemItems()
        {
            if (string.IsNullOrEmpty(m_ProfileAsset.assetPath))
            {
                return;
            }

            Dictionary<string, ParticleSystemResult> resultMapping = m_ProfileAsset.ResultMapping;

            var assetGuids = AssetDatabase.FindAssets("t:prefab", new string[] { m_ProfileAsset.assetPath });
            for (var i = 0; i < assetGuids.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Collect Resources", i + 1 + "/" + assetGuids.Length, i * 1f / assetGuids.Length);

                var guid = assetGuids[i];
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.Contains("."))
                {
                    continue;
                }

                var assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath) as GameObject;
                if (assetObj.GetComponentsInChildren<ParticleSystem>().Length == 0)
                {
                    continue;
                }

                var item = new ParticleSystemAssetItem(assetObj, assetPath);
                if (resultMapping.ContainsKey(assetPath))
                {
                    item.result = resultMapping[assetPath];
                }
                m_Analyst.AddItem(item);
            }
            EditorUtility.ClearProgressBar();
        }

        void InitRules()
        {
            if (m_ProfileAsset.results.Count == 0)
            {
                return;
            }

            foreach (var rule in m_Analyst.rules)
            {
                foreach (var item in m_Analyst.items)
                {
                    rule.Analyze(item);
                }
            }
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Effects Advisor");

            OnSettingsGUI();
            OnAnalysisGUI();
            OnPreviewGUI();
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
                    EditorGUILayout.LabelField("Asset Path:", GUILayout.Width(80));
                    var folder = (DefaultAsset)EditorGUILayout.ObjectField(m_Folder, typeof(DefaultAsset), false);
                    if (folder != m_Folder)
                    {
                        m_Folder = folder;
                        m_ProfileAsset.assetPath = AssetDatabase.GetAssetPath(m_Folder);
                        m_ProfileAsset.results.Clear();

                        m_Analyst.Reset();

                        EditorUtility.SetDirty(m_ProfileAsset);
                        AssetDatabase.SaveAssets();

                        InitParticleSystemItems();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Asset Profile:", GUILayout.Width(80));

                    var settingList = AssetAdvisorSettings.settingList;
                    var settingNames = new string[settingList.Count];
                    var settingIndex = -1;

                    for (var i = 0; i < settingList.Count; i++)
                    {
                        var settings = settingList[i];
                        settingNames[i] = settings.name;
                        if (settings == AssetAdvisorSettings.current)
                        {
                            settingIndex = i;
                        }
                    }

                    var index = EditorGUILayout.Popup(settingIndex == -1 ? 0 : settingIndex, settingNames);
                    if (index != settingIndex)
                    {
                        AssetAdvisorSettings.current = settingList[index];
                        
                        m_ProfileAsset.profile = AssetAdvisorSettings.current.name;
                        EditorUtility.SetDirty(m_ProfileAsset);
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        void OnAnalysisGUI()
        {
            if (m_Folder == null)
            {
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Analysis");

            GUILayout.BeginVertical("Box", GUILayout.Width(50));
            GUILayout.BeginHorizontal();

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Stop", GUILayout.Height(25), GUILayout.Width(50)))
                {
                    EditorApplication.isPlaying = false;
                }
            }
            else
            {
                if (GUILayout.Button("Start", GUILayout.Height(25), GUILayout.Width(50)))
                {
                    OnStart();
                }

                if (GUILayout.Button("Clear", GUILayout.Height(25), GUILayout.Width(50)))
                {
                    m_ProfileAsset.results.Clear();
                    EditorUtility.SetDirty(m_ProfileAsset);
                    AssetDatabase.SaveAssets();

                    InitAnalystItemRules();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        void OnPreviewGUI()
        {
            if (m_Folder == null)
            {
                return;
            }

            var lastRect = GUILayoutUtility.GetLastRect();
            var treeRect = new Rect(8, lastRect.y + 30, 660, 150);
            m_Analyst.treeView.OnGUI(treeRect);
            GUILayout.Space(treeRect.height);

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

            m_Analyst.ShowRuleGUI();

            GUILayout.EndVertical();
        }

        void OnStart()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            EditorUtility.DisplayCancelableProgressBar("Profiling Particle Systems", "Prepare the Environment", 0);

            var guids = AssetDatabase.FindAssets("ParticleSystemPlay t:Scene");
            if (guids.Length > 0)
            {
                var partichePlayScene = AssetDatabase.GUIDToAssetPath(guids[0]);
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(partichePlayScene);
                }
            }

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            EditorApplication.isPlaying = true;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                m_ProfileAsset.results.Clear();

                m_ProfileIndex = 0;
                m_ParticleSystemProgress = new ProgressGUI();
                m_ParticleSystemProgress.total = m_Analyst.items.Count;
                m_ParticleSystemProgress.isCancelable = false;

                ParticleSystemProfile.OnProfilingComplete += OnProfileItemComplete;

                ProfilingParticleSystem();
            }
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                m_ParticleSystemProgress.Clear();
            }
        }

        void ProfilingParticleSystem()
        {
            var item = m_Analyst.items[m_ProfileIndex++];
            var particleGo = item.obj as GameObject;

            var profilingObject = Instantiate(particleGo, new Vector3(0, 0, 0), particleGo.GetComponent<Transform>().rotation);
            profilingObject.SetActive(true);

            var profiling = profilingObject.AddComponent<ParticleSystemProfile>();
            profiling.path = item.path;
            profiling.maxDuration = AnalysistUtility.GetParticleSystemDuration(profilingObject);

            m_ParticleSystemProgress.Show("Profiling Particle Systems", "Playing " + m_ProfileIndex + " / " + m_Analyst.items.Count
                + " (name: " + item.obj.name + " dt: " + profiling.maxDuration + "s)");
        }

        void OnProfileItemComplete(ParticleSystemResult result)
        {
            m_ProfileAsset.results.Add(result);

            if (m_ParticleSystemProgress.cancel || m_ProfileIndex >= m_Analyst.items.Count)
            {
                OnProfileComplete();
            }
            else
            {
                EditorCoroutineUtility.StartCoroutine(DelayAlalysis(), this);
            }
        }

        IEnumerator DelayAlalysis()
        {
            yield return new EditorWaitForSeconds(1);

            if (m_ParticleSystemProgress.cancel)
            {
                yield break;
            }
            else
            {
                ProfilingParticleSystem();
            }
        }

        void OnProfileComplete()
        {
            EditorUtility.SetDirty(m_ProfileAsset);
            AssetDatabase.SaveAssets();

            InitAnalystItemRules();
            ShowNotification(new GUIContent("Analysis of complete!"));

            m_ParticleSystemProgress.Clear();
            EditorApplication.isPlaying = false;
        }
    }
}