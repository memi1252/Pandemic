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
        // 델리게이트 체인 추가
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject obj in PreFabs)
        {
            var dd = Instantiate(obj, obj.transform.position, obj.transform.rotation);
            dd.GetComponent<NetworkObject>().Spawn();
        }
    }

    void OnDisable()
    {
        // 델리게이트 체인 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    
}
