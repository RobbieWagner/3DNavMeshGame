using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface to handle interactable objects
public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    protected Player player;
    protected bool playerCanInteract;
    protected bool isInteracting;
    protected bool runningCooldown;

    protected void Start() 
    {
        player = GameObject.Find("PlayerPos").GetComponent<Player>();
        playerCanInteract = false;
        isInteracting = false;
        runningCooldown = false;
    }

    //Lets player interact when inside of trigger
    protected virtual void OnTriggerEnter(Collider collision)
    {   
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            Debug.Log("can interact");
            playerCanInteract = true;
            player.canInteractWithObjects = true;
        }
    }

    //prevents player interaction outside of trigger
    protected virtual void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        { 
            playerCanInteract = false;
            player.canInteractWithObjects = false;
        }
    }

    //checks for interaction
    protected virtual void Update()
    {
        if(player.canInteractWithObjects && playerCanInteract && Input.GetKeyDown(KeyCode.K))
        {
            Interact();
        }

        if(isInteracting && !runningCooldown)
        {
            StartCoroutine(CoolDownInteraction());
        }
    }

    //Find a child object of a parent
    protected virtual GameObject FindObject(GameObject parent, string name)
    {
        Transform[] children= parent.GetComponentsInChildren<Transform>(true);
        foreach(Transform child in children){
            if(child.name.Equals(name)){
                return child.gameObject;
            }
        }
        return null;
    }

    //Interact with the object
    protected virtual void Interact()
    {
        playerCanInteract = false;
        isInteracting = true;
    }

    //prevents player from interacting again immediately
    protected virtual IEnumerator CoolDownInteraction()
    {
        runningCooldown = true;

        while(isInteracting) yield return null;

        yield return new WaitForSeconds(.4f);
        playerCanInteract = true;

        runningCooldown = false;

        StopCoroutine(CoolDownInteraction());
    }
}

