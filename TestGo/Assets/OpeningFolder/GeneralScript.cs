using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneralScript : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject techTree;
    [SerializeField] private GameObject _pauseScreen;
    public static GeneralScript general;

    void Start()
    {
        general = this;

        Camera.DontDestroyOnLoad(this);
    }

    public void changeScene(String sceneName)
    {
        loadingScreen.SetActive(true);
        loadingScreen.GetComponentInChildren<Slider>().value = 0;
        StartCoroutine(loadLevel(sceneName));
    }

    IEnumerator loadLevel(String sceneName)
    {

        AsyncOperation caregamentu = SceneManager.LoadSceneAsync(sceneName);

        while (!caregamentu.isDone)
        {
            float progresso = caregamentu.progress / 0.9f;
            loadingScreen.GetComponentInChildren<Slider>().value = progresso;

            yield return null;

        }

        loadingScreen.SetActive(false);
    }
} 
