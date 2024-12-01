using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TextInspectSystem
{
    public class TextInspectItem : MonoBehaviour
    {
        public static TextInspectItem Instance { get; private set; }
        [Header("Show Information Selection")]
        [SerializeField] private bool showObjectName;
        [SerializeField] private bool showObjectDetails;
        [SerializeField] private bool playDetailsAudio;
        [SerializeField] public bool Door;
        [SerializeField] public bool shoes;

        [Header("Text Parameters")]
        [SerializeField] public string objectName = "Generic Object";

        [Space(10)] [TextArea] [SerializeField] private string objectDetails = "This is a description, please fill in the inspector";

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
    }
}
