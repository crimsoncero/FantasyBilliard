using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : StaticInstance<MenuManager>
{
    [SerializeField] private Camera _menuCamera;
    [SerializeField] private Canvas _pauseCanvas;
    [SerializeField] private Canvas _mainMenuCanvas;

    public GameManager GM { get { return GameManager.Instance; } }


    public void StartGame()
    {
        _menuCamera.enabled = false;
        _mainMenuCanvas.enabled = false;
        UiManager.Instance.ToggleUI(true);
        GM.StartGame();
    }

    public void Restart()
    {
        _pauseCanvas.enabled = GM.PauseToggle(false);
        GM.StartGame();
    }

    public void Quit()
    {
        UiManager.Instance.ToggleUI(false);
        _menuCamera.enabled = true;
        _mainMenuCanvas.enabled = true;
        _pauseCanvas.enabled = GM.PauseToggle(false);
    }

    public void PauseToggle(bool active)
    {
        _pauseCanvas.enabled = GM.PauseToggle(active);
    }
}

