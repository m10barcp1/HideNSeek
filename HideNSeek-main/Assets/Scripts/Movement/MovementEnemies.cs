using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class MovementEnemies : MonoBehaviour
{
    #region Variable
    private NavMeshAgent nma = null;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float rotateSpeed;
    private Animator anim;
    public bool canMove = true;
    private NavMeshPath path;
    Vector3 targetPosition;
    int currentIndex = 0;
    private Vector3 startPoint;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        nma = this.GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        moveSpeed = 0;
        anim.SetBool("IsMoving", false);
        startPoint = transform.localPosition;
        //caculate first path
        path = new NavMeshPath();
        targetPosition = RandomNavmeshLocation(7f);
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
        
    }
    private void Update()
    {
        var HideCharacter = this.gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = this.gameObject.GetComponent<SeekStateManager>();

        if (!GameManager.instance.EndGame && GameManager.instance.onClick)
        {
            nma.enabled = true;
            
            if (HideCharacter!= null)
            {
                if(!HideCharacter.IsImprisoned)
                {   

                    moveSpeed = 1.5f;
                    SetRandomDestination(moveSpeed);
                }
                else anim.SetBool("IsMoving", false);
            }
            else if(SeekCharacter!= null)
            {
                if (GameManager.instance.StartGame)
                {
                    moveSpeed = 2f;
                    SetRandomDestination(moveSpeed);
                }
            }
        }
        else
        {
            moveSpeed = 0;
            nma.enabled = false;
            anim.SetBool("IsMoving", false);
        }
    }

    public void ResetPositionPlayer() => transform.localPosition = startPoint;
    public void RotateObject(Vector3 target) 
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion rotGoal = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, rotateSpeed * Time.deltaTime);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (!gameObject.CompareTag("SeekCharacter"))
        {
            var HideCharacter = other.GetComponent<HideStateManager>();
            if (HideCharacter != null)
            {
                if (HideCharacter.IsImprisoned)
                {
                    HideCharacter.OutImprison();
                    if (GameManager.instance.SeekEnemies != null)
                    {
                        GameManager.instance.GetSeekPlayer();
                    }
                }
            }
        }
    }

    #region Movement
    private void SetRandomDestination(float speed)
    {
        if ((Vector3.Distance(transform.position, targetPosition) < .1f
            || currentIndex > path.corners.Length - 1))
        {
            nma.enabled = true;
            canMove = false;
            anim.SetBool("IsMoving", false);
            StartCoroutine(CreateANewPath(2f));
            currentIndex = 0;
        }
        if (canMove && currentIndex < path.corners.Length)
        {
            nma.enabled = false;
            anim.SetBool("IsMoving", true);
            RotateObject(path.corners[currentIndex]);
            transform.position = Vector3.MoveTowards(transform.position, path.corners[currentIndex], speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, path.corners[currentIndex]) < .1f)
                currentIndex++;
            StopAllCoroutines();
        }
    }
    IEnumerator CreateANewPath(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        path = new NavMeshPath();
        targetPosition = RandomNavmeshLocation(8f);
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

        //anim.SetBool("IsMoving", true);
        canMove = true;
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
    #endregion
}