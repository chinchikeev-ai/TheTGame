using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class GameMenuUxEnhancer : MonoBehaviour
{
    Canvas canvas;

    IEnumerator Start()
    {
        yield return null;
        canvas = FindMenuCanvas();
        if (canvas == null) yield break;

        EnhanceScreens();
        EnhanceButtons();
        SelectFirstAvailableButton();
    }

    void EnhanceScreens()
    {
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu" };
        foreach (string screenName in names)
        {
            Transform screen = canvas.transform.Find(screenName);
            if (screen == null) continue;
            if (screen.GetComponent<MenuScreenTransition>() == null)
                screen.gameObject.AddComponent<MenuScreenTransition>();
        }
    }

    void EnhanceButtons()
    {
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            if (button.GetComponent<MenuButtonFeedback>() == null)
                button.gameObject.AddComponent<MenuButtonFeedback>();
            if (button.GetComponent<MenuUiAudioFeedback>() == null)
                button.gameObject.AddComponent<MenuUiAudioFeedback>();

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            button.navigation = navigation;
        }
    }

    Canvas FindMenuCanvas()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        return menuCanvas != null ? menuCanvas.GetComponent<Canvas>() : null;
    }

    void SelectFirstAvailableButton()
    {
        if (EventSystem.current == null || canvas == null) return;
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            if (!button.gameObject.activeInHierarchy || !button.interactable) continue;
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            return;
        }
    }
}
