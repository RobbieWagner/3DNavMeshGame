using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform playerPos;

    [SerializeField]
    private Animator playerA;

    private bool inching;
    private bool inchingPart2;

    [SerializeField]
    private float displacement;

    private Vector3 moveDir;

    void Start()
    {
        playerA.enabled = true;
        inching = false;
        inchingPart2 = false;
        moveDir = Vector3.forward;
    }

    //Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0f, 0f, vertical).normalized;

        if (!inching && direction.magnitude >= 0.1f)
        {
            inching = true;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Debug.Log("set direction " + moveDir.ToString());
            StartCoroutine(InchWorm());
        }
        else if(inchingPart2) 
        {
            Debug.Log("get direction " + moveDir.ToString());
            playerPos.Translate(moveDir.normalized * displacement * Time.deltaTime);
        }
    }

     public IEnumerator InchWorm()
     {
        Debug.Log("Inching");
        
        playerA.SetBool("Inching", true);
        yield return new WaitForSeconds(playerA.GetCurrentAnimatorStateInfo(0).length);
        playerA.SetBool("Part2", true);
        inchingPart2 = true;
        yield return new WaitForSeconds(playerA.GetCurrentAnimatorStateInfo(0).length);
        inchingPart2 = false;
        playerA.SetBool("Part2", false);
        playerA.SetBool("Inching", false);

        Debug.Log("Inched");
        
        inching = false;
        StopCoroutine(InchWorm());
     }
}
