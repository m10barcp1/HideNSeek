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
    private void Awake()
    {
        if (!this.gameObject.CompareTag("Player"))
            rb = GetComponent<Rigidbody>();
        IsImprisoned = false;
    }
    public void Imprison()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            Debug.Log(this.gameObject.GetComponent<MovementPlayer>());
            this.gameObject.GetComponent<MovementPlayer>().MovementState(0);
            //GameManager.instance.LoseGameAction();
        }
        else
        {
            this.gameObject.layer = LayerMask.NameToLayer("Imprison");
        }
        IsImprisoned = true;
        jail.transform.position = transform.position;
        jail.SetActive(true);
    }
    public void ResetState()
    {
        
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        jail.SetActive(false);
        if (!this.gameObject.CompareTag("Player"))
            rb.velocity = Vector3.zero;
        if (!IsImprisoned)
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            transform.localPosition = Vector3.zero;
        }
        Debug.Log(this.transform.position);
        transform.GetChild(0).gameObject.SetActive(true);
        IsImprisoned = false;
    }   
    public void TurnOffModel()
    {
        transform.GetChild(0).gameObject.SetActive(false);  
    }

}
