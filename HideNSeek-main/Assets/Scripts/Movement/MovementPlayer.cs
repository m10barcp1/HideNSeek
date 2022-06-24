using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    [Header("Core Value")]
    [SerializeField]
    private float playSpeed = 1f;
    [SerializeField]
    private float rotationSpeed = 5f;
    private float gravity = 9.8f;
    [Header("Control")]
    public DynamicJoystick joystick;
    private Vector3 startPosition;
    private CharacterController _controller;
    private Animator anim;
    private Vector3 move;
    
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }
    void Update()
    {
        var HideCharacter = gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = gameObject.GetComponent<SeekStateManager>();

        if (!GameManager.instance.EndGame && GameManager.instance.onClick)
        {
            _controller.enabled = true;
            _controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
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
        _controller.Move(move * Time.deltaTime * Speed);
        // Rotation
        transform.forward = new Vector3(move.x, 0 , move.z);
        // Animator
        anim.SetBool("IsMoving", true);
        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(move.x), Mathf.Abs(move.z)));

    }
    public void SetStateIdle()
    {
        anim.SetBool("IsMoving", false);
        move = Vector3.zero;
        transform.position = startPosition;
    }   
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