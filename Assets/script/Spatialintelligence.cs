using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Spatialintelligence : MonoBehaviour
{
    public static Spatialintelligence Instance { get; private set; }
    [SerializeField] public Material defaultColor;
    [SerializeField] public Material clickColor;
    [SerializeField] public Sprite[] rounds;
    [SerializeField] private Image wiev;
    
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
    }
}