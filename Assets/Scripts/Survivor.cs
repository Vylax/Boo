using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Survivor : MonoBehaviour
{
    public string lastScene = "";
    public float survivedTime = -1;
    public GUIStyle style;
    
    private static Survivor _instance;

    public static Survivor Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void StartPlaying()
    {
        print("aaaa");
        lastScene = "Menu";
        SceneManager.LoadScene("scene1");
    }

    private void OnGUI()
    {
        if(lastScene == "scene1")
        {
            GUI.Box(new Rect(10, 10, Screen.width - 20, 50), "You are dead", style);

            GUI.Box(new Rect(10, 60, Screen.width - 20, 50), $"You survived for {Mathf.FloorToInt(survivedTime)} seconds", style);

            float buttonHeight = (Screen.height - 130 - 20) / 2f;

            if(GUI.Button(new Rect(10, 130, Screen.width - 20, buttonHeight), "Play Again", style))
            {
                //StartPlaying();
            }

            if (GUI.Button(new Rect(10, 130+10+buttonHeight, Screen.width - 20, buttonHeight), "Quit", style))
            {
                Application.Quit();
            }
        }
        else //im so tired dont judge me :)
        {
            if (lastScene == "")
            {
                GUI.Box(new Rect(10, 10, Screen.width - 20, 110), "Boo", style);

                float buttonHeight = (Screen.height - 130 - 20) / 2f;

                if (GUI.Button(new Rect(10, 130, Screen.width - 20, buttonHeight), "Play", style))
                {
                    /*lastScene = "Menu";
                    SceneManager.LoadScene("scene1");*/
                }

                if (GUI.Button(new Rect(10, 130 + 10 + buttonHeight, Screen.width - 20, buttonHeight), "Quit", style))
                {
                    Application.Quit();
                }
            }
        }
    }

}
