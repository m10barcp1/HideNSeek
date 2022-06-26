using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    #region Variables
    [Header("Core Value")]
    [SerializeField]
    private float playSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 5f;
    private float gravity = 9.8f;
    [Header("Control")]
    public DynamicJoystick joystick;
    public Vector3 startPosition;
    private CharacterController _controller;
    private Animator anim;
    private Vector3 move;
    #endregion
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        if (gameObject.CompareTag("SeekPlayer"))
        {
            startPosition = new Vector3(0, -0.675f, 0);

        }else if (gameObject.CompareTag("HidePlayer"))
        {
            startPosition = new Vector3(0, -0.57f, 0);
        }


    }
    void Update()
    {
        var HideCharacter = gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = gameObject.GetComponent<SeekStateManager>();

        if (!GameManager.instance.EndGame && GameManager.instance.onClick)
        {
            _controller.enabled = true;
            #region Hide Mode
            if (HideCharacter != null)
            {
                SetStateIdle();
                if (!HideCharacter.IsImprisoned)
                {
                    if (Mathf.Abs(joystick.Horizontal) > .1f || Mathf.Abs(joystick.Vertical) > .1f)
                    {
                        float horizontalInput = joystick.Horizontal;
                        float verticalInput = joystick.Vertical;
                        move = new Vector3(horizontalInput,0, verticalInput);
                        MovementState(playSpeed);    
                    }
                    else SetStateIdle();
                }
                else joystick.resetInput();
            }
            #endregion
            #region Seek Mode
            // Seek Mode
            else if (SeekCharacter != null)
            {
                if (GameManager.instance.StartGame)
                {
                    if (Mathf.Abs(joystick.Horizontal) > .1f || Mathf.Abs(joystick.Vertical) > .1f)
                    {
                        float horizontalInput = joystick.Horizontal;
                        float verticalInput = joystick.Vertical;
                        move = new Vector3(horizontalInput, 0, verticalInput);
                        MovementState(playSpeed);
                    }
                    else SetStateIdle();
                }
                else joystick.resetInput();
            }
            #endregion
        }
        else
        {
            joystick.resetInput();  
            _controller.enabled = false;
        }
    }

    public void MovementState(float Speed)
    {
        //Positon
        move.y -= gravity;
        _controller.Move(new Vector3(move.x * Time.deltaTime * Speed, move.y * 2.5f, move.z * Time.deltaTime * Speed));
        // Rotation
        transform.forward = new Vector3(move.x, 0 , move.z);
        // Animator
        anim.SetBool("IsMoving", true);
        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(move.x), Mathf.Abs(move.z)));

    }
    public void SetStateIdle() => anim.SetBool("IsMoving", false);
    public void ResetPositionPlayer() => transform.localPosition = startPosition;
    // Process rescue other player
    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.instance.StateOfGame == GameManager.GameState.hide)
        {
            var HideCharacter = other.GetComponent<HideStateManager>();
            if (HideCharacter != null)
            {
                if (HideCharacter.IsImprisoned)
                {
                    HideCharacter.OutImprison();
                    if (GameManager.instance.SeekEnemies != null)
                    {
                        GameManager.instance.SeekEnemies.GetComponent<SeekStateManager>().DecreaseCharaceterInImprison();
                    }
                }   
            }
        }
    }
}