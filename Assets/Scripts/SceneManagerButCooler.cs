using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// allows us to traverse through scenes with ease :)
/// </summary>
public class SceneManagerButCooler : MonoBehaviour
{
    public Animator fadeScreenAnimator;
    public float sceneLoadDelay = 1.0f;
    public void SceneLoad(int sceneId)
    {
        fadeScreenAnimator.SetTrigger("FadeOut");
        StartCoroutine(LoadWithDelay(sceneId, sceneLoadDelay));
    }
    public void EndGame()
    {
        Application.Quit();
    }
    IEnumerator LoadWithDelay( int sceneId, float delay = 1f)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneId);
        yield return null;
    }
}
