using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu;


    private void Start()
    {
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
        }
    }


    private void OpenMenu()
    {
        TimeScale(0);
        gameMenu.SetActive(true);
    }


    public void ChangeScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void TimeScale(float time)
    {
        Time.timeScale = time;
    }
}
