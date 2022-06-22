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
        //bndFloor = GameObject.Find("floor").GetComponent<Renderer>().bounds;
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
        if (!nma.hasPath)
        {
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
        nma.SetDestination(RandomNavmeshLocation(5f));
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}