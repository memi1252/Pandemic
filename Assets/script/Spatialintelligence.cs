using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Spatialintelligence : MonoBehaviour
{
    public static Spatialintelligence Instance { get; private set; }
    [SerializeField] public Material defaultColor;
    [SerializeField] public Material clickColor;
    [SerializeField] private Material successColor;
    [SerializeField] public Sprite[] rounds;
    [SerializeField] private Image wiev;
    [SerializeField] private GameObject[] buttons;
    private int RoundIndex = 0;
    
    [System.Serializable]
    public struct poscheck
    {
        public bool pos0;
        public bool pos1;
        public bool pos2;
        public bool pos3;
        public bool pos4;
        public bool pos5;
        public bool pos6;
    }
    
    public poscheck poschecksX0;
    public poscheck poschecksX1;
    public poscheck poschecksX2;
    public poscheck poschecksX3;
    public poscheck poschecksX4;
    public poscheck poschecksX5;
    public poscheck poschecksX6;

    private void Awake()
    {
        Instance = this;
        firstStart();
    }

    private void Update()
    {
        if (RoundIndex==0)
        {
            if (poschecksX0.pos0 && !poschecksX1.pos0 && !poschecksX2.pos0 && !poschecksX3.pos0 && !poschecksX4.pos0 && !poschecksX5.pos0 && poschecksX6.pos0
                && poschecksX0.pos1 && poschecksX1.pos1 && !poschecksX2.pos1 && !poschecksX3.pos1 && !poschecksX4.pos1 && poschecksX5.pos1 && poschecksX6.pos1
                && poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && poschecksX5.pos2 && poschecksX6.pos2
                && poschecksX0.pos3 && !poschecksX1.pos3 && !poschecksX2.pos3 && poschecksX3.pos3 && !poschecksX4.pos3 && !poschecksX5.pos3 && poschecksX6.pos3
                && poschecksX0.pos4 && !poschecksX1.pos4 && !poschecksX2.pos4 && poschecksX3.pos4 && !poschecksX4.pos4 && !poschecksX5.pos4 && poschecksX6.pos4
                && poschecksX0.pos5 && poschecksX1.pos5 && poschecksX2.pos5 && poschecksX3.pos5 && poschecksX4.pos5 && poschecksX5.pos5 && poschecksX6.pos5
                && poschecksX0.pos6 && poschecksX1.pos6 && poschecksX2.pos6 && poschecksX3.pos6 && poschecksX4.pos6 && poschecksX5.pos6 && poschecksX6.pos6)
            {
                RoundIndex++;
                Debug.Log(RoundIndex);
                StartCoroutine(success());
            }
        }else if (RoundIndex==1)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && poschecksX2.pos0 && poschecksX3.pos0 && poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && poschecksX1.pos1 && poschecksX2.pos1 && poschecksX3.pos1 && poschecksX4.pos1 && poschecksX5.pos1 && !poschecksX6.pos1
                && poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && poschecksX5.pos2 && poschecksX6.pos2
                && !poschecksX0.pos3 && !poschecksX1.pos3 && !poschecksX2.pos3 && poschecksX3.pos3 && !poschecksX4.pos3 && !poschecksX5.pos3 && !poschecksX6.pos3
                && !poschecksX0.pos4 && !poschecksX1.pos4 && !poschecksX2.pos4 && poschecksX3.pos4 && !poschecksX4.pos4 && !poschecksX5.pos4 && !poschecksX6.pos4
                && !poschecksX0.pos5 && !poschecksX1.pos5 && !poschecksX2.pos5 && poschecksX3.pos5 && !poschecksX4.pos5 && !poschecksX5.pos5 && !poschecksX6.pos5
                && !poschecksX0.pos6 && !poschecksX1.pos6 && poschecksX2.pos6 && !poschecksX3.pos6 && !poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==2)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && !poschecksX2.pos0 && poschecksX3.pos0 && !poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && !poschecksX1.pos1 && poschecksX2.pos1 && poschecksX3.pos1 && poschecksX4.pos1 && !poschecksX5.pos1 && !poschecksX6.pos1
                && !poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && poschecksX5.pos2 &&!poschecksX6.pos2
                && poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && poschecksX4.pos3 && poschecksX5.pos3 && poschecksX6.pos3
                && !poschecksX0.pos4 && poschecksX1.pos4 && poschecksX2.pos4 && poschecksX3.pos4 && poschecksX4.pos4 && poschecksX5.pos4 && !poschecksX6.pos4
                && !poschecksX0.pos5 && poschecksX1.pos5 && poschecksX2.pos5 && !poschecksX3.pos5 && poschecksX4.pos5 && poschecksX5.pos5 && !poschecksX6.pos5
                && !poschecksX0.pos6 && poschecksX1.pos6 && poschecksX2.pos6 && !poschecksX3.pos6 && poschecksX4.pos6 && poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==3)
        {
            if (!poschecksX0.pos0 && poschecksX1.pos0 && !poschecksX2.pos0 && !poschecksX3.pos0 && poschecksX4.pos0 && !poschecksX5.pos0 && poschecksX6.pos0
                && !poschecksX0.pos1 && poschecksX1.pos1 && poschecksX2.pos1 && poschecksX3.pos1 && poschecksX4.pos1 && !poschecksX5.pos1 && poschecksX6.pos1
                && !poschecksX0.pos2 && poschecksX1.pos2 && !poschecksX2.pos2 && poschecksX3.pos2 && !poschecksX4.pos2 && !poschecksX5.pos2 &&poschecksX6.pos2
                && !poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && poschecksX4.pos3 && !poschecksX5.pos3 && poschecksX6.pos3
                && poschecksX0.pos4 && poschecksX1.pos4 && poschecksX2.pos4 && poschecksX3.pos4 && poschecksX4.pos4 && poschecksX5.pos4 && poschecksX6.pos4
                && poschecksX0.pos5 && !poschecksX1.pos5 && poschecksX2.pos5 && !poschecksX3.pos5 && poschecksX4.pos5 && !poschecksX5.pos5 && !poschecksX6.pos5
                && poschecksX0.pos6 && !poschecksX1.pos6 && poschecksX2.pos6 && !poschecksX3.pos6 && poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==4)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && !poschecksX2.pos0 && !poschecksX3.pos0 && !poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && poschecksX1.pos1 && poschecksX2.pos1 && !poschecksX3.pos1 && poschecksX4.pos1 && poschecksX5.pos1 && !poschecksX6.pos1
                && !poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && !poschecksX3.pos2 && poschecksX4.pos2 && poschecksX5.pos2 && !poschecksX6.pos2
                && !poschecksX0.pos3 && !poschecksX1.pos3 && !poschecksX2.pos3 && !poschecksX3.pos3 && !poschecksX4.pos3 && !poschecksX5.pos3 && !poschecksX6.pos3
                && !poschecksX0.pos4 && poschecksX1.pos4 && !poschecksX2.pos4 && !poschecksX3.pos4 && !poschecksX4.pos4 && poschecksX5.pos4 && !poschecksX6.pos4
                && !poschecksX0.pos5 && !poschecksX1.pos5 && poschecksX2.pos5 && poschecksX3.pos5 && poschecksX4.pos5 && !poschecksX5.pos5 && !poschecksX6.pos5
                && !poschecksX0.pos6 && !poschecksX1.pos6 && !poschecksX2.pos6 && !poschecksX3.pos6 && !poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==5)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && !poschecksX2.pos0 && poschecksX3.pos0 && !poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && poschecksX0.pos1 && poschecksX1.pos1 && poschecksX2.pos1 && poschecksX3.pos1 && poschecksX4.pos1 && poschecksX5.pos1 && poschecksX6.pos1
                && !poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && poschecksX5.pos2 && !poschecksX6.pos2
                && !poschecksX0.pos3 && !poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && poschecksX4.pos3 && !poschecksX5.pos3 && !poschecksX6.pos3
                && !poschecksX0.pos4 && poschecksX1.pos4 && poschecksX2.pos4 && poschecksX3.pos4 && poschecksX4.pos4 && poschecksX5.pos4 && !poschecksX6.pos4
                && poschecksX0.pos5 && !poschecksX1.pos5 && !poschecksX2.pos5 && !poschecksX3.pos5 && !poschecksX4.pos5 && !poschecksX5.pos5 && poschecksX6.pos5
                && !poschecksX0.pos6 && !poschecksX1.pos6 && !poschecksX2.pos6 && !poschecksX3.pos6 && !poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==6)
        {
            if (!poschecksX0.pos0 && poschecksX1.pos0 && poschecksX2.pos0 && poschecksX3.pos0 && poschecksX4.pos0 && poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && !poschecksX1.pos1 && !poschecksX2.pos1 && poschecksX3.pos1 && !poschecksX4.pos1 && !poschecksX5.pos1 && !poschecksX6.pos1
                && !poschecksX0.pos2 && !poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && !poschecksX5.pos2 && !poschecksX6.pos2
                && !poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && poschecksX4.pos3 && poschecksX5.pos3 && !poschecksX6.pos3
                && poschecksX0.pos4 && !poschecksX1.pos4 && !poschecksX2.pos4 && poschecksX3.pos4 && !poschecksX4.pos4 && !poschecksX5.pos4 && poschecksX6.pos4
                && poschecksX0.pos5 && !poschecksX1.pos5 && !poschecksX2.pos5 && poschecksX3.pos5 && !poschecksX4.pos5 && !poschecksX5.pos5 && poschecksX6.pos5
                && !poschecksX0.pos6 && poschecksX1.pos6 && poschecksX2.pos6 && !poschecksX3.pos6 && poschecksX4.pos6 && poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==7)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && poschecksX2.pos0 && poschecksX3.pos0 && poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && poschecksX1.pos1 && poschecksX2.pos1 && !poschecksX3.pos1 && poschecksX4.pos1 && poschecksX5.pos1 && !poschecksX6.pos1
                && poschecksX0.pos2 && poschecksX1.pos2 && poschecksX2.pos2 && poschecksX3.pos2 && poschecksX4.pos2 && !poschecksX5.pos2 && !poschecksX6.pos2
                && poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && !poschecksX4.pos3 && !poschecksX5.pos3 && !poschecksX6.pos3
                && poschecksX0.pos4 && poschecksX1.pos4 && poschecksX2.pos4 && poschecksX3.pos4 && poschecksX4.pos4 && !poschecksX5.pos4 && !poschecksX6.pos4
                && !poschecksX0.pos5 && poschecksX1.pos5 && poschecksX2.pos5 && poschecksX3.pos5 && poschecksX4.pos5 && poschecksX5.pos5 && !poschecksX6.pos5
                && !poschecksX0.pos6 && !poschecksX1.pos6 && poschecksX2.pos6 && poschecksX3.pos6 && poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==8)
        {
            if (!poschecksX0.pos0 && !poschecksX1.pos0 && !poschecksX2.pos0 && !poschecksX3.pos0 && !poschecksX4.pos0 && !poschecksX5.pos0 && !poschecksX6.pos0
                && !poschecksX0.pos1 && poschecksX1.pos1 && !poschecksX2.pos1 && !poschecksX3.pos1 && !poschecksX4.pos1 && poschecksX5.pos1 && !poschecksX6.pos1
                && poschecksX0.pos2 && poschecksX1.pos2 && !poschecksX2.pos2 && !poschecksX3.pos2 && !poschecksX4.pos2 && poschecksX5.pos2 && poschecksX6.pos2
                && poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && poschecksX3.pos3 && poschecksX4.pos3 && poschecksX5.pos3 && poschecksX6.pos3
                && poschecksX0.pos4 && poschecksX1.pos4 && !poschecksX2.pos4 && !poschecksX3.pos4 && !poschecksX4.pos4 && poschecksX5.pos4 && poschecksX6.pos4
                && !poschecksX0.pos5 && poschecksX1.pos5 && !poschecksX2.pos5 && !poschecksX3.pos5 && !poschecksX4.pos5 && poschecksX5.pos5 && !poschecksX6.pos5
                && !poschecksX0.pos6 && !poschecksX1.pos6 && !poschecksX2.pos6 && !poschecksX3.pos6 && !poschecksX4.pos6 && !poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
        else if (RoundIndex==9)
        {
            if (!poschecksX0.pos0 && poschecksX1.pos0 && poschecksX2.pos0 && poschecksX3.pos0 && poschecksX4.pos0 && poschecksX5.pos0 && !poschecksX6.pos0
                && poschecksX0.pos1 && !poschecksX1.pos1 && poschecksX2.pos1 && poschecksX3.pos1 && poschecksX4.pos1 && !poschecksX5.pos1 && poschecksX6.pos1
                && poschecksX0.pos2 && poschecksX1.pos2 && !poschecksX2.pos2 && poschecksX3.pos2 && !poschecksX4.pos2 && poschecksX5.pos2 && poschecksX6.pos2
                && poschecksX0.pos3 && poschecksX1.pos3 && poschecksX2.pos3 && !poschecksX3.pos3 && poschecksX4.pos3 && poschecksX5.pos3 && poschecksX6.pos3
                && poschecksX0.pos4 && poschecksX1.pos4 && !poschecksX2.pos4 && poschecksX3.pos4 && !poschecksX4.pos4 && poschecksX5.pos4 && poschecksX6.pos4
                && poschecksX0.pos5 && !poschecksX1.pos5 && poschecksX2.pos5 && poschecksX3.pos5 && poschecksX4.pos5 && !poschecksX5.pos5 && poschecksX6.pos5
                && !poschecksX0.pos6 && poschecksX1.pos6 && poschecksX2.pos6 && poschecksX3.pos6 && poschecksX4.pos6 && poschecksX5.pos6 && !poschecksX6.pos6)
            {
                RoundIndex++;
                StartCoroutine(success());
            }
        }
    }

    private void gameReset()
    {
        poschecksX0.pos0 =false;
        poschecksX0.pos1 =false;
        poschecksX0.pos2 =false;
        poschecksX0.pos3 =false;
        poschecksX0.pos4 =false;
        poschecksX0.pos5 =false;
        poschecksX0.pos6 =false;
        poschecksX1.pos0 =false;
        poschecksX1.pos1 =false;
        poschecksX1.pos2 =false;
        poschecksX1.pos3 =false;
        poschecksX1.pos4 =false;
        poschecksX1.pos5 =false;
        poschecksX1.pos6 =false;
        poschecksX2.pos0 =false;
        poschecksX2.pos1 =false;
        poschecksX2.pos2 =false;
        poschecksX2.pos3 =false;
        poschecksX2.pos4 =false;
        poschecksX2.pos5 =false;
        poschecksX2.pos6 =false;
        poschecksX3.pos0 =false;
        poschecksX3.pos1 =false;
        poschecksX3.pos2 =false;
        poschecksX3.pos3 =false;
        poschecksX3.pos4 =false;
        poschecksX3.pos5 =false;
        poschecksX3.pos6 =false;
        poschecksX4.pos0 =false;
        poschecksX4.pos1 =false;
        poschecksX4.pos2 =false;
        poschecksX4.pos3 =false;
        poschecksX4.pos4 =false;
        poschecksX4.pos5 =false;
        poschecksX4.pos6 =false;
        poschecksX5.pos0 =false;
        poschecksX5.pos1 =false;
        poschecksX5.pos2 =false;
        poschecksX5.pos3 =false;
        poschecksX5.pos4 =false;
        poschecksX5.pos5 =false;
        poschecksX5.pos6 =false;
        poschecksX6.pos0 =false;
        poschecksX6.pos1 =false;
        poschecksX6.pos2 =false;
        poschecksX6.pos3 =false;
        poschecksX6.pos4 =false;
        poschecksX6.pos5 =false;
        poschecksX6.pos6 =false;

        foreach (GameObject dd in buttons)
        {
            dd.GetComponent<Renderer>().material = defaultColor;
        }
    }
    
    IEnumerator success()
    {
        foreach (GameObject dd in buttons)
        {
            dd.GetComponent<Renderer>().material = successColor;
        }
        yield return new WaitForSeconds(1.5f);
        gameReset();
        if(RoundIndex < rounds.Length)
        {
            wiev.sprite = rounds[RoundIndex];
        }
        else
        {
            Debug.Log("일단 여기가 끝");
        }
        
    }

    private void firstStart()
    {
        wiev.sprite = rounds[0];
        wiev.color = new Color(255, 255, 255, 255);
    }
}