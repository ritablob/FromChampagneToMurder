using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Singleton that allows us to traverse through scenes with ease :)
/// </summary>
public class SceneManagerButCooler : MonoBehaviour
{
    private static SceneManagerButCooler _instance;

    public static SceneManagerButCooler Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SceneLoad(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
