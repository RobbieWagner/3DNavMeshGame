using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    // Used to change scenes 

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private Image uiImage;
    [SerializeField]
    private Canvas screenCoverCanvas;

    [SerializeField]
    private bool changeSceneOnStart;

    [SerializeField]
    Canvas loadingScreen;

    private void Start() 
    {
        if(changeSceneOnStart)
        StartCoroutine(TransitionScene(sceneName));

        if(loadingScreen != null)loadingScreen.enabled = false;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);    
    }
        
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);    
    }

    public void RunTransitionScene()
    {
        StartCoroutine(TransitionScene());
    }

    public void RunTransitionScene(string scene)
    {
        StartCoroutine(TransitionScene(scene));
    }

    public IEnumerator TransitionScene()
    {
        
        screenCoverCanvas.gameObject.SetActive(true);

        while(uiImage.color.a < 1f)
        {
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, uiImage.color.a + .1f);
            yield return new WaitForSeconds(.25f);
        }
        ChangeScene();
        StopCoroutine(TransitionScene());
    }

    public IEnumerator TransitionScene(string scene)
    {
        screenCoverCanvas.gameObject.SetActive(true);

        while(uiImage.color.a < 1f)
        {
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, uiImage.color.a + .1f);
            yield return new WaitForSeconds(.25f);
        }
        loadingScreen.enabled = true;
        ChangeScene(scene);
        StopCoroutine(TransitionScene(scene));
    }
}
