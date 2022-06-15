using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public AnimationCurve curve;
    public Transform A, B;
    private Vector3 _offset;
    [SerializeField] private Transform target;
    public Transform HidePlayer, SeekPlayer;
    [SerializeField] private float smoothTime;
    private Vector3 _currentVelocity = Vector3.zero;
    float currentTime;
    bool checkMoveAtStartGame;
    private void Awake()
    {
        _offset = B.position - target.position;
        checkMoveAtStartGame = false;
    }
    void Start()
    {
        currentTime = 0;

        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.StateOfGame == GameManager.GameState.hide)
        {
            target = HidePlayer.transform;
        }
        else
        {
            target = SeekPlayer.transform;
        }
        if (GameManager.instance.onClick && !checkMoveAtStartGame)
        {
            currentTime += Time.deltaTime;
            float Speed = curve.Evaluate(currentTime);
            transform.position = Vector3.MoveTowards(transform.position, B.position, Speed);
        }
        if(B.position == transform.position)
        {
            checkMoveAtStartGame = true;
        }
        if (checkMoveAtStartGame)
        {
            Debug.Log("Follow");
            Vector3 targetPosition = target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);

        }
    }

}
