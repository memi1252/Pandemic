using System;
using UnityEngine;

public class TP : MonoBehaviour
{
    [SerializeField] private int index;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (index == 0)
                other.transform.position = new Vector3(-12.5f, 0, 466);
            else if (index == 3)
                other.transform.position = new Vector3(2.066f, 1, 542.327f);
            else if (index == 31)
                other.transform.position = new Vector3(-6, 1.069f, 454.53f);
            else if (index == 4)
                other.transform.position = new Vector3(12.86f, 1.068f, 445.814f);
            else if (index == 41)
                other.transform.position = new Vector3(-6, 1.069f, 454.53f);
        }
    }
}