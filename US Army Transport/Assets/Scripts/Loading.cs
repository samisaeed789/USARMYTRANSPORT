using DG.Tweening;
using UIAnimatorDemo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{

    [SerializeField] Image loadingImage;
    AsyncOperation asyncLoad;
    
    void Start()
    {
        StartLoadingFill("MainMenu");
    }
    public void StartLoadingFill(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        DOTween.To(() => 0, value => loadingImage.fillAmount = value, 1f, 8f)
                .SetEase(Ease.Linear)
                .OnKill(() => OnLoadingCompleteFill());
    }
    void OnLoadingCompleteFill()
    {
        asyncLoad.allowSceneActivation = true;
    }
}
