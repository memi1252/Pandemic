using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private float rotCamXAxisSpeed = 5f;
    [SerializeField] private float rotCamYAxisSpeed = 3f;
    [SerializeField] private GameObject camera;

    public float eulerAngleX;
    public float eulerAngleY;
    public float x = 0f;
    public float z = 0f;

    public bool anglepause = true;
    public bool pause; 
    public float limitMaxX = 50;
    public readonly float limitMinX = -80;
    public static RotateToMouse Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        cursor();
    }

    public void CalculateRotation(float mouseX, float mouseY)
    {
        if (anglepause)
        {
            eulerAngleY += mouseX * rotCamYAxisSpeed;
            eulerAngleX -= mouseY * rotCamYAxisSpeed;
            eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
            transform.rotation = Quaternion.Euler(0, eulerAngleY, z);
            camera.transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, z);
        }
    }

    

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;

        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private void cursor()
    {
        if (pause)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}