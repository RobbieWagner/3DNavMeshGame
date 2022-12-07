using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Canvas thisCanvas;

    private bool pressingEsc;
    private bool runningCooldown;

    // Start is called before the first frame update
    void Start()
    {
        thisCanvas.enabled = false;
        pressingEsc = false;
        runningCooldown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            pressingEsc = true;

            if(thisCanvas.enabled == false)
            {
                thisCanvas.enabled = true;
                Time.timeScale = 0;
            }
            else
            {
                thisCanvas.enabled = false;
                Time.timeScale = 1;
            }
        }

        if(pressingEsc && !runningCooldown)
        StartCoroutine(CooldownPause());
    }

    IEnumerator CooldownPause()
    {
        runningCooldown = true;
        yield return new WaitForSeconds(1f);
        pressingEsc = false;
        runningCooldown = false;
        StopCoroutine(CooldownPause());
    }
}
