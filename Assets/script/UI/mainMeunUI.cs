using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMeunUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        startButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LobbySecen");
        });

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
