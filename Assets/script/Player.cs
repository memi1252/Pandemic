using System;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;
    [SerializeField] private RotateToMouse rotateToMouse;
    [SerializeField]private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rotateToMouse = GetComponent<RotateToMouse>();
    }

    private void Update()
    {
        Move();
        RoomRotate();
        UpdateRotate();
    }

    private void Move()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
        }
        
        var moveDir = new Vector3(horizontalInput, 0, verticalInput);
        
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
    
    private void UpdateRotate()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        
        
        rotateToMouse.CalculateRotation(mouseX, mouseY);
    }

    private void RoomRotate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.A))
        {
            Physics.gravity = -transform.right * 9.81f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.D))
        {
            Physics.gravity = transform.right * 9.81f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.W))
        {
            Physics.gravity = transform.forward * 9.81f;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.S))
        {
            Physics.gravity = -transform.forward * 9.81f;
        }
    }
}
