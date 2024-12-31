using System;
using UnityEngine;

public class OnOffUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameMenuUI.Instance.gameObject.activeSelf)
            {
                GameMenuUI.Instance.Hide();
                SettingUI.Instance.Hide();
            }
            else
            {
                GameMenuUI.Instance.Show();
            }
        }
    }
}
