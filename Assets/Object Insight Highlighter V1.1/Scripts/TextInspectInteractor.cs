﻿using System;
using UnityEngine;
using Unity.Netcode;

namespace TextInspectSystem
{
    public class TextInspectInteractor : MonoBehaviour
    {
        public static TextInspectInteractor Instance { get; private set; }
        [Header("Raycast Features")]
        [SerializeField] private float rayLength = 5;
        private Camera _camera;

        private TextInspectItem textItem;
        public GameObject tagetName;

        [Header("Input Key")]
        [SerializeField] private KeyCode interactKey;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            if (!TryGetComponent<Camera>(out _camera))
            {
                Debug.LogError("Camera component not found on the GameObject.");
            }
        }

        private void Update()
        {
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayLength))
            {
                tagetName = hit.collider.gameObject;
                var readableItem = hit.collider.GetComponent<TextInspectItem>();
                if (readableItem != null)
                {
                    textItem = readableItem;
                    textItem.ShowObjectName(true);
                    HighlightCrosshair(true);
                }
                else
                {
                    ClearText();
                }
            }
            else
            {
                ClearText();
            }

            if (textItem != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    textItem.ShowDetails();
                    HandleInteraction(hit.collider.gameObject);
                }
        }
        }

        void ClearText()
        {
            if (textItem != null)
            {
                textItem.ShowObjectName(false);
                HighlightCrosshair(false);
                textItem = null;
            }
        }

        void HighlightCrosshair(bool on)
        {
            TextInspectUIManager.instance.HighlightCrosshair(on);
        }

        void HandleInteraction(GameObject clickedObject)
        {
            var shoesScript = clickedObject.GetComponent<Shoes>();
            if (shoesScript != null)
            {
                var textInspectItem = clickedObject.GetComponent<TextInspectItem>();
                if (textInspectItem != null && textInspectItem.shoes)
                {
                    var player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>();
                    if (player != null)
                    {
                        shoesScript.GravitationCore = player.transform.GetChild(2).gameObject;
                        player.SetShoes(true);
                        if (shoesScript.GravitationCore != null)
                        {
                            shoesScript.Interaction();
                        }
                        else
                        {
                            Debug.LogError("GravitationCore is not assigned.");
                        }
                    }
                }
            }
        }
    }
}