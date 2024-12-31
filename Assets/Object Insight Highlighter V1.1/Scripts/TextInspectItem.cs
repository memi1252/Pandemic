using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TextInspectSystem
{
    public class TextInspectItem : NetworkBehaviour
    {
        public static TextInspectItem Instance { get; private set; }
        [Header("Show Information Selection")]
        [SerializeField] public bool showObjectName;
        [SerializeField] public bool showObjectDetails;
        [SerializeField] private bool playDetailsAudio;
        [SerializeField] public bool Door;
        [SerializeField] public bool shoes;
        [SerializeField] public bool gloves;
        [SerializeField] public bool heavy;
        [SerializeField] public GameObject Prefabs = null;
        [SerializeField] public Material material = null;
        [SerializeField] public bool stone;
        [SerializeField] public int stoneNumber;
        [SerializeField] public int isStonePutNumber;
        [SerializeField] public bool StoneCase;
        [SerializeField] public bool PlayerDead;
        [SerializeField] public bool tp;
        [SerializeField] public string tpName;

        [System.Serializable]
        public struct memory
        {
            public bool start;
            public bool reset;
            public bool red;
            public bool ornage;
            public bool yellow;
            public bool green;
            public bool blue;
            public bool darkblue;
            public bool purple;
            public bool brown;
            public bool pick;
        }
        [Header("memory")]
        public memory memorys;
        public bool ismemory;

        [Header("SpatialIntelligenceAbility")] 
        public bool isSpatialIntelligenceAbility;
        public int posX;
        public int posY;

        [Header("Text Parameters")]
        [SerializeField] public string objectName = "Generic Object";

        [Space(10)] [TextArea] [SerializeField] public string objectDetails = "This is a description, please fill in the inspector";

        [Header("Audio Parameters")]
        [SerializeField] private AudioClip detailsAudioClip;
        private AudioSource audioSource;
        bool inclick;
        float time = 5;
        
        public GameObject shoesScript;

        void Awake()
        {
            Instance = this;
            // Ensure there's an AudioSource component and set it up
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void ShowObjectName(bool showName)
        {
            if(showObjectName)
            {
                TextInspectUIManager.instance.ShowName(objectName, showName);
            }
        }

        public void ShowDetails()
        {
            if (showObjectDetails)
            {
                TextInspectUIManager.instance.ShowObjectDetails(objectDetails);

                // Play audio if enabled
                if (playDetailsAudio && detailsAudioClip != null)
                {
                    audioSource.clip = detailsAudioClip;
                    audioSource.Play();
                }
            }
            
        }
        
        public void coolTimeBar(bool on)
        {
            TextInspectUIManager.instance.CoolTimeBar(on);
        }
        
        
        public bool ShowCoolTimeBarCkeck()
        {
            return TextInspectUIManager.instance.ShowCoolTimeBarCkeck();
        }
    }
}
