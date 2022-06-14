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

    private void Awake()
    {
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
        IsImprisoned = false;
        this.gameObject.layer = LayerMask.NameToLayer("HideMask");
        jail.SetActive(false);
        this.transform.position = Vector3.zero;
        transform.GetChild(0).gameObject.SetActive(true);
    }   
    public void TurnOffModel()
    {
        transform.GetChild(0).gameObject.SetActive(false);  
    }

}
