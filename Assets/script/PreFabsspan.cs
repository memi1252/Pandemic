using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreFabsspan : MonoBehaviour
{
    [SerializeField] private GameObject[] PreFabs =null;

    void OnEnable()
    {
        // Add delegate chain
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (GameObject obj in PreFabs)
            {
                var dd = Instantiate(obj, obj.transform.position, obj.transform.rotation);
                dd.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    void OnDisable()
    {
        // Remove delegate chain
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
