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
    public GameObject FootPrintLeft;
    public GameObject FootPrintRight;
    public int footStep;
    public bool FootPrintMode;
    public LayerMask GroundMask;
    float SeekPlayerRadius;
    private void Awake()
    {
        if (!this.gameObject.CompareTag("Player"))
            rb = GetComponent<Rigidbody>();
        IsImprisoned = false;
        FootPrintMode = false;
        footStep = 0;
    }
    private void Update()
    {
        if (GameManager.instance.StateOfGame == GameManager.GameState.seek &&
            !gameObject.CompareTag("Player"))
        {
            float SeekPlayerRadius = SeekPlayer.GetComponent<SeekStateManager>().viewRadius;
            if (Vector3.Distance(transform.position, SeekPlayer.transform.position) <= SeekPlayerRadius)
            {
                FootPrintMode = true;
            }
            else
            {
                footStep = 0;
                FootPrintMode = false;
            }
        }
        else
        {
            footStep = 0;
            FootPrintMode = false;
        }
    }
    public void Imprison()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            GameManager.instance.LoseGameAction();
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
        //if (FootPrintMode)
        //{
            RaycastHit hit;
            if (Physics.Raycast(LeftFootLocation.position, LeftFootLocation.forward, out hit))
            {
                FootPrintLeft.transform.GetChild(footStep % 2).transform.position = hit.transform.position; 
                FootPrintLeft.transform.GetChild(footStep % 2).gameObject.SetActive(true);
                //GameObject footPrint = Instantiate(FootPrintLeft, hit.point + hit.normal * .05f, Quaternion.LookRotation(hit.normal, LeftFootLocation.up));
                //footPrint.transform.rotation = new Quaternion(0, 0, 0, 0);
                FootPrintLeft.transform.GetChild(footStep % 2).transform.rotation = transform.rotation;
                StartCoroutine(PoolingFootPrint(2f, FootPrintLeft.transform.GetChild(footStep % 2).gameObject));
                
            }
            footStep++;
        //}
    }
    public void RightFootIns()
    {
        //if (FootPrintMode)
        //{
            RaycastHit hit;
            if (Physics.Raycast(RightFootLocation.position, RightFootLocation.forward, out hit))
            {
                FootPrintRight.transform.GetChild(footStep % 2).transform.position = hit.transform.position;
                FootPrintRight.transform.GetChild(footStep % 2).gameObject.SetActive(true);
                //GameObject footPrint = Instantiate(FootPrintRight, hit.point + hit.normal * .05f, Quaternion.LookRotation(hit.normal, RightFootLocation.up));
                //footPrint.transform.rotation = new Quaternion(0, 0, 0, 0);
                FootPrintRight.transform.GetChild(footStep % 2).transform.rotation = transform.rotation;
                StartCoroutine(PoolingFootPrint(1f, FootPrintLeft.transform.GetChild(footStep % 2).gameObject));
               
            }
            footStep++;
        //}
    }
    IEnumerator PoolingFootPrint(float seconds, GameObject footPrint)
    {
        yield return new WaitForSeconds(seconds);
        //Destroy(footPrint);
        footPrint.SetActive(false);
    }
}
