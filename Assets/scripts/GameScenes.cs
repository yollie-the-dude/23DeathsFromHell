using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScenes : MonoBehaviour
{
    public void LoadScene(int sceneID)
    {
        StartCoroutine(Change(sceneID));

    }
    IEnumerator Change(int sceneID)
    {
        yield return new WaitForSeconds(0.15f);
        SceneManager.LoadScene(sceneID);
    }

    public void quit()
    {
        Application.Quit();
    }
}
