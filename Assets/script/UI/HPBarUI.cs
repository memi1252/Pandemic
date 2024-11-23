using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image HP;
    [SerializeField] private Image HPBack;
    [SerializeField] private Button demege;
    private int dd;
    private void Awake()
    {
        dd = Random.Range(1, 11);
        demege.onClick.AddListener(() =>
        {
            Player.Instance.playerHP -= dd;
        });
    }

    private void Update()
    {
        dd = Random.Range(1, 11);
        if (Player.Instance != null)
        {
            HP.fillAmount = Player.Instance.playerHP / 100f;
            if (Player.Instance.playerHP < 95 && Player.Instance.playerHP > 0)
            {
                HPBack.fillAmount = HP.fillAmount + 0.05f;
            }
            else if (Player.Instance.playerHP <= 0)
            {
                HPBack.fillAmount = 0;
            }
        }
    }
}
