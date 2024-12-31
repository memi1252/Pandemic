using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Image HP;
    [SerializeField] private Image HPBack;

    private Player player;

    public void SetPlayer(Player player)
    {
        this.player = player;
        UpdateHP(player.playerHP.Value);
    }

    public void UpdateHP(int playerHP)
    {
        HP.fillAmount = playerHP / 100f;
        if (playerHP < 95 && playerHP > 0)
        {
            HPBack.fillAmount = HP.fillAmount + 0.05f;
        }
        else if (playerHP <= 0)
        {
            HPBack.fillAmount = 0;
        }
    }
}