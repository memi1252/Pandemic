using System;
using Unity.Mathematics;
using UnityEngine;
using Object = System.Object;

public class landPlayer : MonoBehaviour
{

    public static landPlayer Instance { get; private set; }

    public enum graf
    {
        up,
        down,
        left,
        right,
        forward,
        back
    }

    [SerializeField] public graf gravity;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private ConstantForce constantForce;

    private void Awake()
    {
        Instance = this;
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("point"))
        {
            rigidbody = other.gameObject.transform.parent.GetComponent<Rigidbody>();
            constantForce = other.gameObject.transform.parent.GetComponent<ConstantForce>();
            switch (gravity)
            {
                case graf.up:
                    if (Player.Instance.up == false)
                    {
                        if (Player.Instance.right)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-90, 90, 0);
                        }
                        else if (Player.Instance.forward)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-90, 0, 0);
                        }
                        else if (Player.Instance.back)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-90, 180, 0);
                        }
                        else if (Player.Instance.left)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-90, -90, 0);
                        }
                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(0, 9.81f +9.81f, 0);
                        //rigidbody.AddForce(0, 9.81f + 9.81f, 0);
                        //Physics.gravity = new Vector3(0, 9.81f, 0);
                        Player.Instance.up = true;
                        Player.Instance.down = false;
                        Player.Instance.left = false;
                        Player.Instance.right = false;
                        Player.Instance.forward = false;
                        Player.Instance.back = false;
                    }

                    break;
                case graf.down:
                    if (Player.Instance.down == false)
                    {
                        if (Player.Instance.right)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(90, -90, 0);
                        }
                        else if (Player.Instance.forward)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(90, 180, 0);
                        }
                        else if (Player.Instance.back)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(90, 0, 0);
                        }
                        else if (Player.Instance.left)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(90, 90, 0);
                        }
                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(0, 0, 0);
                        //rigidbody.AddForce(0, -9.81f - 9.81f, 0);
                        //Physics.gravity = new Vector3(0, -9.81f, 0);
                        Player.Instance.up = false;
                        Player.Instance.down = true;
                        Player.Instance.left = false;
                        Player.Instance.right = false;
                        Player.Instance.forward = false;
                        Player.Instance.back = false;
                    }

                    break;
                case graf.left:
                    if (Player.Instance.left == false)
                    {
                        if (Player.Instance.down)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, -90, 0);
                        }
                        else if (Player.Instance.forward)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, -90, 90);
                        }
                        else if (Player.Instance.back)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, -90, -90);
                        }
                        else if (Player.Instance.up)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(180, 90, 0);
                        }

                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(-9.81f, 9.81f, 0);
                        //rigidbody.AddForce(-9.81f- 9.81f, 0, 0);
                        //Physics.gravity = new Vector3(-9.81f, 0, 0);
                        Player.Instance.up = false;
                        Player.Instance.down = false;
                        Player.Instance.left = true;
                        Player.Instance.right = false;
                        Player.Instance.forward = false;
                        Player.Instance.back = false;
                    }

                    break;
                case graf.right:
                    if (Player.Instance.right == false)
                    {
                        if (Player.Instance.down)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (Player.Instance.forward)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 90, -90);
                        }
                        else if (Player.Instance.back)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 90, 90);
                        }
                        else if (Player.Instance.up)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(180, -90, 0);
                        }

                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(+9.81f, 9.81f, 0);
                        //rigidbody.AddForce(9.81f +9.81f, 0, 0);
                        //Physics.gravity = new Vector3(+9.81f, 0, 0);
                        Player.Instance.up = false;
                        Player.Instance.down = false;
                        Player.Instance.left = false;
                        Player.Instance.right = true;
                        Player.Instance.forward = false;
                        Player.Instance.back = false;
                    }

                    break;
                case graf.forward:
                    if (Player.Instance.forward == false)
                    {
                        if (Player.Instance.right)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 0, 90);
                        }
                        else if (Player.Instance.up)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-180, -180, 0);
                        }
                        else if (Player.Instance.down)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
                        }
                        else if (Player.Instance.left)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 0, -90);
                        }
                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(0, 9.81f, 9.81f);
                        //rigidbody.AddForce(0, 0, 9.81f+9.81f);
                        //Physics.gravity = new Vector3(0, 0, +9.81f);
                        Player.Instance.up = false;
                        Player.Instance.down = false;
                        Player.Instance.left = false;
                        Player.Instance.right = false;
                        Player.Instance.forward = true;
                        Player.Instance.back = false;
                    }

                    break;
                case graf.back:
                    if (Player.Instance.back == false)
                    {
                        if (Player.Instance.right)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(180, 0, 90);
                        }
                        else if (Player.Instance.up)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(-180, 0, 0);
                        }
                        else if (Player.Instance.down)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                        }
                        else if (Player.Instance.left)
                        {
                            other.gameObject.transform.parent.rotation = Quaternion.Euler(180, 0, -90);
                        }
                        other.gameObject.transform.parent.Rotate(-90, 0, 0);
                        constantForce.force = new Vector3(0, 9.81f, -9.81f);
                        //rigidbody.AddForce(0, 0, -9.81f -9.81f);
                        //Physics.gravity = new Vector3(0, 0, -9.81f);
                        Player.Instance.up = false;
                        Player.Instance.down = false;
                        Player.Instance.left = false;
                        Player.Instance.right = false;
                        Player.Instance.forward = false;
                        Player.Instance.back = true;
                    }
                    break;
            }
        }
    }
}
