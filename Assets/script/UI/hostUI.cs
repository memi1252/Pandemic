using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class hostUI : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button join;
    [SerializeField] private GameObject[] PreFabs =null;

    private void Awake()
    {
        host.onClick.AddListener(() =>
        {
            Debug.Log("Host");
            
            Cursor.lockState = CursorLockMode.Locked;
            NetworkManager.Singleton.StartHost();
            foreach (GameObject obj in PreFabs)
            {
                var dd = Instantiate(obj, obj.transform.position, obj.transform.rotation);
                dd.GetComponent<NetworkObject>().Spawn();
                if (dd.name == "glove")
                {
                    dd.transform.position = new Vector3(4.39f, 0.6285198f, 6.292754f);
                }

                if (dd.name == "Cube")
                {
                    dd.transform.position = new Vector3(-4.06f, 2,5);
                }
            }
            
        });
        join.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            Cursor.lockState = CursorLockMode.Locked;
            NetworkManager.Singleton.StartClient();
        });
    }
}
