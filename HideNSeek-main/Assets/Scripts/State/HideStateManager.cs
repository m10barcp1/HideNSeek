using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
public  class HideStateManager : MonoBehaviour
{

    //public static HideStateManager instance;
    [Header("Core Value")]
    public bool IsImprisoned;
    public GameObject jail;
    public Rigidbody rb;
    public GameObject SeekPlayer;

    [Header("Foot Print")]
    public Transform RightFootLocation;
    public Transform LeftFootLocation;
    public bool FootPrintMode;
    public LayerMask GroundMask;
    private void Awake()
    {
        if (!this.gameObject.CompareTag("Player"))
            rb = GetComponent<Rigidbody>();
        IsImprisoned = false;
        FootPrintMode = false;
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
                FootPrintMode = false;
            }
        }
        else
        {
            FootPrintMode = false;
        }
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
    public void TurnOffModel()
    {
        transform.GetChild(0).gameObject.SetActive(false);  
    }

    #region Process Foot Print
    public void LeftFootIns()
    {
        //if (FootPrintMode)
        //{
            RaycastHit hit;
            if (Physics.Raycast(LeftFootLocation.position, 
                new Vector3(LeftFootLocation.position.x, LeftFootLocation.position.y -.5f, LeftFootLocation.position.z), out hit,GroundMask))
            {
                Debug.Log("Left");   
                PoolingFootPrint.instance.FootPrint(hit.point, transform);
            }
        //}
    }
    
    public void RightFootIns()
    {
        //if (FootPrintMode)
        //{
        Debug.Log("Collide");
        RaycastHit hit;
            if (Physics.Raycast(RightFootLocation.position, 
                new Vector3(RightFootLocation.position.x, RightFootLocation.position.y -.4f, RightFootLocation.position.z), out hit,GroundMask))
            {
                Debug.Log("Right");
                PoolingFootPrint.instance.FootPrint(hit.point, transform);      
            }
        //}
    }
    #endregion
    #region Process IsImprisoned
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
    public void OutImprison()
    {
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        jail.SetActive(false);
        IsImprisoned = false;
    }
    #endregion


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(RightFootLocation.position ,new Vector3(RightFootLocation.position.x, RightFootLocation.position.y  -.5f, RightFootLocation.position.z));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(LeftFootLocation.position, new Vector3(LeftFootLocation.position.x, LeftFootLocation.position.y  -.5f, LeftFootLocation.position.z));
    }
}
