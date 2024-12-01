using System;
using TextInspectSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoorUI : MonoBehaviour
{
    /*[SerializeField] private GameObject Bar;
    [SerializeField] private Image doorBar;
    [SerializeField] private GameObject doorNameUI;
    [SerializeField] private GameObject door;

    private bool open;
    private float doorTime = 3f;
    [SerializeField] private float doorTimer = 3f;

    private void Update()
    { 
        var textInspectItem = TextInspectInteractor.Instance.tagetName.GetComponent<TextInspectItem>();
        if (textInspectItem != null && textInspectItem.Door )
        {
            if (Input.GetButton("E"))
            {
                if (doorNameUI.gameObject.activeSelf)
                {
                    if (textInspectItem.objectName == "Door")
                    {
                        Bar.SetActive(true);
                        doorTime -= Time.deltaTime;
                        doorBar.fillAmount = (doorTimer - doorTime) / doorTimer;
                    }
                }
                else
                {
                    Bar.SetActive(false);
                    doorBar.fillAmount = 0;
                }
            }
            else
            {
                Bar.SetActive(false);
                doorBar.fillAmount = 0;
                doorTime = doorTimer;
            }
        }

        if (doorBar.fillAmount == 1)
        {
            if (!open)
            {
                Debug.Log("Door Open");
                doorBar.fillAmount = 0;
                Bar.SetActive(false);
                door.transform.rotation = Quaternion.Euler(0, 90, 0);
                doorTime = doorTimer;
                open = true;
            }
            else
            {
                Debug.Log("Door close");
                doorBar.fillAmount = 0;
                Bar.SetActive(false);
                door.transform.rotation = Quaternion.Euler(0, 0, 0);
                doorTime = doorTimer;
                open = false;
            }
        }
    }*/
}