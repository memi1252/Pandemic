using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    static public PlayerCamera Instance { get; private set; }

    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public GameObject playerCamera;

    public float xRotation = 0f;
    public float yRotation = 0f;

    private void Awake()
    {
        Instance = this;

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
        xRotation = 0f;
    }

    private void Start()
    {
        if (!IsOwner)
        {
            playerCamera.SetActive(false);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 30f);
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(90 + xRotation,-90, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}