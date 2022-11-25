using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Transform playerPos;
    [SerializeField]
    private Transform head;
    [SerializeField]
    private float headRotationMin = -30f;
    [SerializeField]
    private float headRotationMax = 30f;

    [SerializeField]
    private TextMeshProUGUI errorText;

    [SerializeField]
    private Animator playerA;

    private bool inching;
    private bool inchingPart2;

    private bool flashingText;

    [SerializeField]
    private LayerMask ignoreLayers;

    [HideInInspector]
    public float checkDistanceBehind;
    [HideInInspector]
    public float checkDistanceForward;
    [SerializeField]
    public float displacement;

    [SerializeField]
    private CinemachineFreeLook cameraRigs;

    private Vector3 moveDir;

    void Start()
    {
        playerA.enabled = true;
        inching = false;
        inchingPart2 = false;
        moveDir = Vector3.forward;

        flashingText = false;

        checkDistanceBehind = (displacement + .1f) * 2;
        checkDistanceForward = displacement + .1f;
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
        else if(!inching)
        {
            head.rotation = Quaternion.Euler(0f, 0f, 90f);//Mathf.Clamp(cam.rotation.y, headRotationMin + transform.rotation.y, headRotationMax + transform.rotation.y));
        }
        
        playerPos.position = new Vector3(playerPos.position.x, .1f, playerPos.position.z);

        //.1f + ((playerPos.localScale.y - 1f)/4)
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

        if(Physics.Raycast(ray, out hit, checkDistance))
        {
            if(hit.collider.gameObject.tag.Equals("Obstacle")) return false;
        }
        
        return true;
    }

    public void changeWormSize(float change)
    {
        displacement += change;
        checkDistanceBehind = (displacement + .1f) * 2;
        checkDistanceForward = displacement + .1f;
        cameraRigs.m_Orbits[2].m_Radius += change * 4;
        cameraRigs.m_Orbits[2].m_Height += change * .4f;

        //doesn't work right now (need to fix issues with y pos)
        //playerPos.position += new Vector3(0, change/10, 0);
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
