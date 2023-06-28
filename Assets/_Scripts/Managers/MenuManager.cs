using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : StaticInstance<MenuManager>
{
    [SerializeField] private Camera _menuCamera;



    public GameManager GM { get { return GameManager.Instance; } }


    public void StartGame()
    {
        _menuCamera.enabled = false;
        GM.StartGame();
    }


}
