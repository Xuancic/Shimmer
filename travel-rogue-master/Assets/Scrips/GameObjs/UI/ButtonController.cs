using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject map;
    public GameObject bag;
    public GameObject grid;
    // private Player player;
    private bool isactive=false;





    public void OnClick()
    {
        isactive = !isactive;
        map.SetActive(isactive);
        bag.SetActive(isactive);
        grid.SetActive(isactive);
        if (isactive)
        {
            GameManager.Instance.player.Controller.Controlable = false;
        }
        else
        {
            GameManager.Instance.player.Controller.Controlable = true;
        }
    }
}
