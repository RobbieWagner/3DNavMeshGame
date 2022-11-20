using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdiblePlant : Interactable
{
    [SerializeField]
    private float growth;
    private Vector3 sizeChange;

    private Transform PlayerPos;

    void Start()
    {
        PlayerPos = GameObject.Find("PlayerPos").transform;

        sizeChange = new Vector3(growth, growth, growth);
    }

    protected override void Interact()
    {
        PlayerPos.localScale += sizeChange;
        player.canInteractWithObjects = false;
    
        playerTPM.changeWormSize(growth);

        Destroy(gameObject);
    }
}
