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
    private Animator playerA;
    [SerializeField]
    private string inchingAnimation;

    private bool inching;

    [SerializeField]
    private float displacement;

    void Start()
    {
        playerA.enabled = false;
        inching = false;
    }

    //Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0f, 0f, vertical).normalized;

        if (!inching && direction.magnitude >= 0.1f)
        {
            Debug.Log("Preparing to inch");
            inching = true;

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            StartCoroutine(InchWorm(inchingAnimation, moveDir, displacement));
        }
    }

     public IEnumerator InchWorm(string inchingAnimation, Vector3 moveDir, float displacement)
     {
        Debug.Log("Inching");

        playerA.enabled = true;
        playerA.Play(inchingAnimation);
        yield return new WaitForSeconds(playerA.GetCurrentAnimatorStateInfo(0).length - 0.01f);
        animation[inchingAnimation].time = 0f;
        playerA.enabled = false;

        Debug.Log("Inched");
        controller.Move(moveDir.normalized * displacement);
        
        inching = false;
        StopCoroutine(InchWorm(inchingAnimation, moveDir, displacement));
     }
}