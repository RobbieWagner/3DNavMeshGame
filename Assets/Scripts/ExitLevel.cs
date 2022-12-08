using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{

    [SerializeField]
    private string sceneName;

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag.Equals("Player")) 
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
