using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform playerPos;

    [SerializeField]
    private TextMeshProUGUI errorText;

    [SerializeField]
    private Animator playerA;

    private bool inching;
    private bool inchingPart2;

    private bool flashingText;

    [SerializeField]
    private LayerMask ignoreLayers;
    [SerializeField]
    private float checkDistanceBehind;
    [SerializeField]
    private float checkDistanceForward;
    [SerializeField]
    private float displacement;

    private Vector3 moveDir;

    void Start()
    {
        playerA.enabled = true;
        inching = false;
        inchingPart2 = false;
        moveDir = Vector3.forward;

        flashingText = false;
    }

    //Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(0f, 0f, vertical).normalized;

        if (!inching && direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if(checkMovability(-moveDir.normalized, checkDistanceBehind) && checkMovability(moveDir.normalized, checkDistanceForward))
            {
                inching = true;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                StartCoroutine(InchWorm());
            }
            else if(!flashingText) StartCoroutine(flashText());
        }
        else if(inchingPart2) 
        {
            controller.Move(moveDir.normalized * displacement * Time.deltaTime);
        }
    }

    private IEnumerator flashText()
    {
        flashingText = true;
        errorText.text = "Object blocking movement";
        yield return new WaitForSeconds(3f);
        errorText.text = "";
        flashingText = false;
        StopCoroutine(flashText());
    }

    private bool checkMovability(Vector3 direction, float checkDistance)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);
        return !Physics.Raycast(ray, out hit, checkDistance);
    }

    public IEnumerator InchWorm()
    {
        playerA.SetBool("Inching", true);
        yield return new WaitForSeconds(playerA.GetCurrentAnimatorStateInfo(0).length);
        playerA.SetBool("Part2", true);
        inchingPart2 = true;
        yield return new WaitForSeconds(playerA.GetCurrentAnimatorStateInfo(0).length);
        inchingPart2 = false;
        playerA.SetBool("Part2", false);
        playerA.SetBool("Inching", false);
        
        inching = false;
        StopCoroutine(InchWorm());
    }
}
