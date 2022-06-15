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
        if (!GameManager.instance.EndGame && GameManager.instance.onClick)
        {
            _controller.enabled = true;
            if (HideCharacter != null)
            {
                if (!HideCharacter.IsImprisoned)
                {
                    MovementState(playSpeed);
                }
            }
            else if (SeekCharacter != null)
            {
                if (GameManager.instance.StartGame)
                {
                    MovementState(2.5f);
                }
            }
        }
        else
        {
            _controller.enabled = false;

        }
        


    }
    public void MovementState(float Speed)
    {
        
        float horizontalInput = joystick.Horizontal;
        float verticalInput = joystick.Vertical;
        Vector3 move = new Vector3(horizontalInput, 0, verticalInput);
        if (Mathf.Abs(horizontalInput) > .1f || Mathf.Abs(verticalInput) > .1f)
        {
            move.y -= gravity*Time.deltaTime;
            _controller.Move(move * Time.deltaTime * Speed);
            if (Speed != 0)
            {
                gameObject.transform.forward = new Vector3(horizontalInput, 0, verticalInput);
                anim.SetBool("IsMoving", true);
                anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput)));
            }
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
                if(HideCharacter.IsImprisoned)
                HideCharacter.ResetState();
            }
        }
    }
}