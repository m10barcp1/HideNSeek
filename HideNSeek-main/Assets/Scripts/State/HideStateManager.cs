using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public  class HideStateManager : MonoBehaviour
{
    #region Variables
    //public static HideStateManager instance;
    [Header("Core Value")]
    public bool IsImprisoned;
    public GameObject jail;
    public Rigidbody rb;
    public GameObject SeekPlayer;
    public GameObject StateImg;
    private int NumberOfRescue;
    public TextMeshProUGUI textNumberOfRescue;

    [Header("Foot Print")]
    public Transform RightFootLocation;
    public Transform LeftFootLocation;
    public bool FootPrintMode;
    public LayerMask GroundMask;

    #endregion
    private void Awake()
    {
        if (!gameObject.CompareTag("SeekPlayer") || !gameObject.CompareTag("HidePlayer"))
            rb = GetComponent<Rigidbody>();
        IsImprisoned = false;
        FootPrintMode = false;
    }
    private void Start()
    {
        NumberOfRescue = 0;
    }
    private void Update()
    {
        if (GameManager.instance.StateOfGame == GameManager.GameState.seek &&
           (!gameObject.CompareTag("HidePlayer")))
        {
            float SeekPlayerRadius = SeekPlayer.GetComponent<SeekStateManager>().viewRadius;
            if (Vector3.Distance(transform.position, SeekPlayer.transform.position) <= SeekPlayerRadius)
            {
                FootPrintMode = true;
            }
            else
            {
                FootPrintMode = false;
            }
        }
        else
        {
            FootPrintMode = false;
        }
    }

    #region Funcion State
    public void ResetState()
    {
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        transform.GetChild(0).gameObject.SetActive(true);
        
        if (!gameObject.CompareTag("HidePlayer"))
        {
            StateImg.transform.GetChild(1).gameObject.SetActive(true);
            rb.velocity = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }else
        {
            ResetValueRescue();
        }
           
        transform.localRotation = new Quaternion(0, 0, 0, 0);
        
        TurnOnModel();
        jail.SetActive(false);
        IsImprisoned = false;
        
    }
    public void TurnOffModel() => transform.GetChild(0).gameObject.SetActive(false);
    public void TurnOnModel() => transform.GetChild(0).gameObject.SetActive(true);

    #endregion
    #region Process Foot Print
    public void LeftFootIns()
    {
        if (FootPrintMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(LeftFootLocation.position, 
                new Vector3(LeftFootLocation.position.x, LeftFootLocation.position.y -.5f, LeftFootLocation.position.z), out hit,GroundMask))
            {   
                PoolingFootPrint.instance.FootPrint(hit.point, transform);
            }
        }
    }
    
    public void RightFootIns()
    {
        if (FootPrintMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(RightFootLocation.position, 
                new Vector3(RightFootLocation.position.x, RightFootLocation.position.y -.4f, RightFootLocation.position.z), out hit,GroundMask))
            {
                PoolingFootPrint.instance.FootPrint(hit.point, transform);      
            }
        }
    }
    #endregion
    #region Process IsImprisoned
    public void Imprison()
    {
        if (gameObject.CompareTag("HidePlayer"))
        {
            GameManager.instance.LoseGameAction();
        }
        else
        {
            TurnOffModel();
            StateImg.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.layer = LayerMask.NameToLayer("Imprison");
        }
        IsImprisoned = true;
        jail.transform.position = transform.position;
        jail.SetActive(true);
    }
    public void OutImprison()
    {
        gameObject.layer = LayerMask.NameToLayer("HideMask");
        if (!gameObject.CompareTag("HidePlayer"))
        {
            TurnOnModel();
            StateImg.transform.GetChild(1).gameObject.SetActive(true);
        }
        
        jail.SetActive(false);
        IsImprisoned = false;
    }
    #endregion

    #region Process number of rescue 
    public void ResetValueRescue()
    {
        if (gameObject.CompareTag("HidePlayer"))
        {
            NumberOfRescue = 0;
            textNumberOfRescue.text = NumberOfRescue.ToString();
        }        
    }
    public void IncreaseValueRescue()
    {
        if (gameObject.CompareTag("HidePlayer"))
        {
            NumberOfRescue++;
            textNumberOfRescue.text = NumberOfRescue.ToString();
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(RightFootLocation.position ,new Vector3(RightFootLocation.position.x, RightFootLocation.position.y  -.5f, RightFootLocation.position.z));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(LeftFootLocation.position, new Vector3(LeftFootLocation.position.x, LeftFootLocation.position.y  -.5f, LeftFootLocation.position.z));
    }
}
