using EditorKit;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace UPA
{
    [CustomEditor(typeof(ParticleSystemProfile))]
    public class ParticleSystemProfileInspector : Editor
    {

        [MenuItem("GameObject/Effects/Profiler", false, 11)]
        private static void EffectProfile()
        {
            var go = Selection.activeGameObject;
            var particleSystemRenderer = go.GetComponentsInChildren<ParticleSystemRenderer>(true);
            if (particleSystemRenderer.Length == 0)
            {
                return;
            }

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            
            EditorApplication.isPlaying = true;
        }

    }
}