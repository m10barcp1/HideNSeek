using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    private float playSpeed = 4f;
    private float gravity = 9.8f;
    private CharacterController _controller;
    private Animator anim;
    public DynamicJoystick joystick;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        //joystick = GameObject.Find("Dynamic Joystick").GetComponent<DynamicJoystick>();

    }
    void Update()
    {
        var HideCharacter = this.gameObject.GetComponent<HideStateManager>();
        var SeekCharacter = this.gameObject.GetComponent<SeekStateManager>();
        if (HideCharacter != null)
        {
            if (!HideCharacter.IsImprisoned)
            {
                MovementState(playSpeed);
            } 
        }
        else
        {
            if (GameManager.instance.StartGame)
            {
                MovementState(0.5f);
            }
        }


    }
    private bool onGround()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, .5f);
    }
    public void MovementState(float Speed)
    {
        
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;
        float _gravity = 0;
        Vector3 move = new Vector3(horizontalInput, 0, verticalInput);
        if (!onGround())
        {
            _gravity -= gravity * Time.deltaTime;
            move.y = _gravity;
            _controller.Move(move);
        }
        if (Mathf.Abs(horizontalInput) > .1f || Mathf.Abs(verticalInput) > .1f)
        {
            _controller.Move(move * Time.deltaTime * Speed);
        }
        if (Mathf.Abs(horizontalInput) > .1f || Mathf.Abs(verticalInput) > .1f)
        {
            gameObject.transform.forward = new Vector3(horizontalInput, 0, verticalInput);
            anim.SetBool("IsMoving", true);
            anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));
        }
        else
        {
            SetStateIdle();
        }
    }
    public void SetStateIdle()
    {
        anim.SetBool("IsMoving", false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(GameManager.instance.StateOfGame == GameManager.GameState.hide)
        {
            var HideCharacter = other.GetComponent<HideStateManager>();
            if (HideCharacter != null)
            {
                HideCharacter.ResetState();
            }
        }
    }
}