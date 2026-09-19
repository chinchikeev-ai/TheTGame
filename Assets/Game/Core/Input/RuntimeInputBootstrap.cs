using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public sealed class RuntimeInputBootstrap : MonoBehaviour
{
    public EventSystem EventSystem { get; private set; }

    void Awake()
    {
        EnsureRuntimeInput();
    }

    public void EnsureRuntimeInput()
    {
        EventSystem = FindFirstObjectByType<EventSystem>();
        if (EventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            EventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

#if ENABLE_INPUT_SYSTEM
        InputSystemUIInputModule inputModule = EventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
            inputModule = EventSystem.gameObject.AddComponent<InputSystemUIInputModule>();

        BaseInputModule[] modules = EventSystem.GetComponents<BaseInputModule>();
        for (int i = 0; i < modules.Length; i++)
            modules[i].enabled = modules[i] == inputModule;

        inputModule.AssignDefaultActions();
#else
        StandaloneInputModule inputModule = EventSystem.GetComponent<StandaloneInputModule>();
        if (inputModule == null)
            inputModule = EventSystem.gameObject.AddComponent<StandaloneInputModule>();

        BaseInputModule[] modules = EventSystem.GetComponents<BaseInputModule>();
        for (int i = 0; i < modules.Length; i++)
            modules[i].enabled = modules[i] == inputModule;
#endif

        RuntimeFileLogger.Event("INPUT", "Runtime EventSystem ready.");
    }
}
