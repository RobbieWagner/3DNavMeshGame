using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Class keeps track of variables called by multiple classes

    [HideInInspector]
    public bool canInteractWithObjects;

    [HideInInspector]
    public bool isReadingDialogue;

    void Start()
    {
        canInteractWithObjects = false;

        isReadingDialogue = false;
    }
}
