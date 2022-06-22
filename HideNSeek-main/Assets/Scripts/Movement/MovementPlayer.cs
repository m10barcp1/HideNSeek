using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    [SerializeField]
    private float playSpeed = 0.5f;
    [SerializeField]
    private float rotationSpeed = 5f;
    private float gravity = 9.8f;
    private CharacterController _controller;
    private Animator anim;
    public DynamicJoystick joystick;
    public Vector3 move;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();   
    }
    void Update()
    {
        var HideCharacter = gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = gameObject.GetComponent<SeekStateManager>();

        if (!GameManager.instance.EndGame && GameManager.instance.onClick)
        {
            _controller.enabled = true;
            _controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            
            if (HideCharacter != null)
            {
                //Debug.Log($"{Mathf.Abs(verticalInput)} - Vertical  +  {Mathf.Abs(horizontalInput)} - Horizontal  ");
                SetStateIdle();
                if (!HideCharacter.IsImprisoned)
                {
                    
                    if (Mathf.Abs(joystick.Horizontal) > .1f || Mathf.Abs(joystick.Vertical) > .1f)
                    {
                        float horizontalInput = joystick.Horizontal;
                        float verticalInput = joystick.Vertical;
                        Debug.Log("MoveHide");
                        move = new Vector3(horizontalInput,0, verticalInput);
                        MovementState(playSpeed);
                        
                    }
                    else
                    {
                        SetStateIdle();
                    }
                }
                else
                {
                    joystick.resetInput();
                }
            }
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
                    else
                    {
                        SetStateIdle();
                    }
                }
                else
                {
                    joystick.resetInput();
                }
            }
        }
        else
        {
            joystick.resetInput();  
            _controller.enabled = false;

        }
        
                

    }
    public void MovementState(float Speed)
    {
        
        move.y -= gravity;
        _controller.Move(move * Time.deltaTime * Speed);
        transform.forward = new Vector3(move.x, 0 , move.z);
        anim.SetBool("IsMoving", true);
        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(move.x), Mathf.Abs(move.z)));
        //joystick = new DynamicJoystick();

    }
    public void SetStateIdle()
    {
        anim.SetBool("IsMoving", false);
        move = Vector3.zero;
    }   
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