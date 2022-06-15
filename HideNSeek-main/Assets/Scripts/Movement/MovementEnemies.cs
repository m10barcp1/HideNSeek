using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MovementEnemies : MonoBehaviour
{
    private NavMeshAgent nma = null;
    private Bounds bndFloor;
    private Vector3 moveto;
    private bool flag = false;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        nma = this.GetComponent<NavMeshAgent>();
        bndFloor = GameObject.Find("floor").GetComponent<Renderer>().bounds;
    }
    private void Start()
    {
        nma.speed = 0;
        anim.SetBool("IsMoving", false);
        SetRandomDestination();
    }
    private void Update()
    {
        var HideCharacter = this.gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = this.gameObject.GetComponent<SeekStateManager>();

        if (GameManager.instance.onClick && !GameManager.instance.EndGame)
        {
            nma.enabled = true;
            if (HideCharacter!= null)
            {
                CharacterMovement(2f);
            }
            else if(SeekCharacter!= null)
            {
                if (GameManager.instance.StartGame)
                {
                    CharacterMovement(3f);
                }
            }
        }
        else
        {
            nma.enabled = false;
            anim.SetBool("IsMoving", false);
        }
    }
    public void CharacterMovement(float moveSpeed)
    {
        var HideCharacter = this.gameObject.GetComponent<HideStateManager>();
        nma.speed = moveSpeed;
        anim.SetBool("IsMoving", true);
        anim.SetFloat("Speed", nma.speed);
        if (!nma.hasPath && !flag)
        {
            flag = true;
            SetRandomDestination();
        }
        if (HideCharacter != null)
        {
            if (HideCharacter.IsImprisoned)
            {
                nma.speed = 0;
                anim.SetBool("IsMoving", false);
            }
        }
    }
    private void SetRandomDestination()
    {
        //1. pick a point
        float rx = Random.Range(bndFloor.min.x, bndFloor.max.x);
        float rz = Random.Range(bndFloor.min.z, bndFloor.max.z);
          
        moveto = new Vector3(rx, this.transform.position.y, rz);

        Vector3 movementDirection = new Vector3(moveto.x, 0, moveto.z);
        movementDirection.Normalize();
        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 4f * Time.deltaTime);
        nma.SetDestination(moveto);
        Invoke("CheckPointOnPath", 0.2f);
        flag = false;
    }
    private void CheckPointOnPath()
    {
        if (nma.pathEndPosition != moveto)
        {
            SetRandomDestination();
        }
    }
}