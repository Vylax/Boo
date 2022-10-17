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
        }
    }

}
