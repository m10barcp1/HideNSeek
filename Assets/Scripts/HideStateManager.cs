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
    private NavMeshAgent nma;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else if (instance != this)
        //{
        //    Destroy(instance);
        //}
        IsImprisoned = false;
        nma = this.GetComponent<NavMeshAgent>(); 

    }
    // Update is called once per frame
    public void Imprison()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Imprison");
        IsImprisoned = true;
        jail.transform.position = transform.position;
        jail.SetActive(true);
    }

}
