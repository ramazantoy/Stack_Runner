using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Current;
    private Scene _lastLoadedScene;
    void Start()
    {
        Current = this;
        GameObject.FindObjectOfType<AdController>().InitializeAds();
        ChangeLevel("Level "+PlayerPrefs.GetInt("currentlevel"));
     
    }
    public void ChangeLevel(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));
    }
    IEnumerator ChangeScene(string sceneName)//farklı zaman diliminde olan işlemlerin bitmesini beklemek amacıyla kullanacağız
    {
        if (_lastLoadedScene.IsValid())//sahnemiz yüklümü  yüklü ise silip yenisini yülüycez
        {
            SceneManager.UnloadSceneAsync(_lastLoadedScene);
            bool sceneUnloaded = false;
            while (!sceneUnloaded)
            {
                sceneUnloaded = !_lastLoadedScene.IsValid();
                yield return new WaitForEndOfFrame();
            }
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);//diğer sahneleri silip bu sahneyi yüklemesin diye additive kullandık(üzerlerine eklemek amacıyla)
        bool sceneLoaded = false;
        while (!sceneLoaded)
        {
            _lastLoadedScene = SceneManager.GetSceneByName(sceneName);
            sceneLoaded = _lastLoadedScene != null && _lastLoadedScene.isLoaded;
            yield return new WaitForEndOfFrame();
        }
    }

}
