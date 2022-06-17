using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
public  class HideStateManager : MonoBehaviour
{

    //public static HideStateManager instance;
    
    public bool IsImprisoned;
    public GameObject jail;
    public Rigidbody rb;

    public GameObject SeekPlayer;
    public Transform RightFootLocation;
    public Transform LeftFootLocation;
    public GameObject FootPrint;
    public bool FootPrintMode;
    public LayerMask GroundMask;
    private void Awake()
    {
        if (!this.gameObject.CompareTag("Player"))
            rb = GetComponent<Rigidbody>();
        IsImprisoned = false;
        FootPrintMode = false;
    }
    public void Imprison()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.Log(this.gameObject.GetComponent<MovementPlayer>());
            gameObject.GetComponent<MovementPlayer>().MovementState(0);
            //GameManager.instance.LoseGameAction();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Imprison");
        }
        IsImprisoned = true;
        jail.transform.position = transform.position;
        jail.SetActive(true);
    }
    public void ResetState()
    {
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        transform.GetChild(0).gameObject.SetActive(true);
        
        if (!this.gameObject.CompareTag("Player"))
            rb.velocity = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localPosition = Vector3.zero;
        jail.SetActive(false);
        IsImprisoned = false;
    }
    public void OutImprison()
    {
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        jail.SetActive(false);
        IsImprisoned = false;
    }
    public void TurnOffModel()
    {
        transform.GetChild(0).gameObject.SetActive(false);  
    }

    public void LeftFootIns()
    {
        if (FootPrintMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(LeftFootLocation.position, LeftFootLocation.forward, out hit, GroundMask))
            {
                GameObject footPrint =  Instantiate(FootPrint, hit.point + hit.normal * .05f, Quaternion.LookRotation(hit.normal, LeftFootLocation.up));
                footPrint.transform.rotation = new Quaternion(0, 0, 0, 0);
                //footPrint.transform.Rotate(new Vector3(90,0,0), Space.World); 
                //GameObject footPrint = Instantiate(FootPrint,hit.transform);
                StartCoroutine(PoolingFootPrint(2f, footPrint));

            }
        }
    }
    public void RightFootIns()
    {
        if (FootPrintMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(RightFootLocation.position, RightFootLocation.forward, out hit, GroundMask))
            {
                GameObject footPrint =  Instantiate(FootPrint, hit.point+hit.normal*.05f, Quaternion.LookRotation(hit.normal, RightFootLocation.up));
                //footPrint.transform.Rotate(new Vector3(90, 0, 0), Space.World);
                footPrint.transform.rotation = new Quaternion(0, 0, 0, 0);
                //GameObject footPrint = Instantiate(FootPrint);
                StartCoroutine(PoolingFootPrint(2f, footPrint));
            }

        }
    }
    private void Update()
    {
        if(GameManager.instance.StateOfGame == GameManager.GameState.seek &&
            !gameObject.CompareTag("Player"))
        {
            float SeekPlayerRadius = SeekPlayer.GetComponent<SeekStateManager>().viewRadius;
            if (Vector3.Distance(transform.position, SeekPlayer.transform.position) <= SeekPlayerRadius)
            {
                FootPrintMode = true;
            }
            else
            {
                FootPrintMode = false;
            }
        }
        else
        {
            FootPrintMode = false;
        }
    }

    IEnumerator PoolingFootPrint(float seconds, GameObject footPrint)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(footPrint);
    }
}
