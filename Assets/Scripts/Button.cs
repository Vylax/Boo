using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public enum btnType
    {
        Play,
        Quit
    }

    public btnType btn;

    public void FixedOnMouseDown()
    {
        if(btn == btnType.Play)
        {
            Survivor.Instance.StartPlaying();
        }
        else
        {
            Application.Quit();
        }
    }
}
