#nullable enable
using UnityEngine;

namespace Platonic.UIToolkit
{
    /// <summary>
    /// Ticks <see cref="BindingRunner"/> once per frame. Runs in LateUpdate after other scripts so that state
    /// changed this frame is visible in the UI the same frame, ahead of UI Toolkit's own panel update.
    /// In the editor outside play mode, <c>EditorApplication.update</c> drives it for editor windows.
    /// </summary>
    [DefaultExecutionOrder(short.MaxValue)]
    internal sealed class BindingDriver : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Create()
        {
            var go = new GameObject("Platonic.BindingDriver") { hideFlags = HideFlags.HideAndDontSave };
            DontDestroyOnLoad(go);
            go.AddComponent<BindingDriver>();
        }

        private void LateUpdate()
        {
            BindingRunner.UpdateAll();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void HookEditorUpdate()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                if (!Application.isPlaying)
                {
                    BindingRunner.UpdateAll();
                }
            };
        }
#endif
    }
}
