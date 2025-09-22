using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralScript : MonoBehaviour
{
    [SerializeField] private GameObject _Canvas;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject techTree;
    [SerializeField] private GameObject _pauseScreen;
    public static GeneralScript general;

    void Start()
    {
        general = this;
        //Camera.DontDestroyOnLoad(_Canvas);
        Camera.DontDestroyOnLoad(this);
    }

    public void changeScene(String sceneName)
    {

        StartCoroutine(loadLevel(sceneName));
    }

    IEnumerator loadLevel(String sceneName)
    {

        AsyncOperation caregamentu = SceneManager.LoadSceneAsync(sceneName);

        while (!caregamentu.isDone)
        {
            float progresso = caregamentu.progress;
            Debug.Log(progresso);
            yield return null;
        }

    }
}

public static class techTreee
{
    
}
