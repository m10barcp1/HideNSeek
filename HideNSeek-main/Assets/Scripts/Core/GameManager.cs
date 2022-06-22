using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        hide,
        seek
    }
    public GameState StateOfGame;

    [Header("Setting")]
    public GameObject Player;
    public GameObject SeekEnemies;
    public GameObject[] Character;
    //private List<Vector3> InitTransform;
    [HideInInspector]
    public bool onClick = false;
    [HideInInspector]
    public bool StartGame, WinGame, LoseGame, EndGame;

    [Header("Panel To Show")]
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject MenuPanel;

    private float TimePlay, TimeStartUp;
    [Header("Text")]
    public Text CountDownTime;
    public Text CountDownToStartUp;

    [Header("Core")]
    public GameObject joystick;
    public GameObject Camera;

    [Header("Button")]
    public GameObject HideBtn;
    public GameObject SeekBtn;
    

    private int indexCurentLevel;
    private Transform InitTransform;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        indexCurentLevel = 1;
    }
    private void Start()
    {
        joystick.SetActive(false);
        InitTransform = Player.transform;
    }
    private void Update()
    {
        if (onClick)
        {
            var SeekPlayer = Player.transform.GetChild(0).GetComponent<SeekStateManager>();
            var HidePlayer = Player.transform.GetChild(1).GetComponent<HideStateManager>();
            CountDownToStartUp.enabled = true;
            TimeStartUp -= Time.deltaTime;
            CountDownToStartUp.text = Mathf.Round(TimeStartUp).ToString();
            
            if(TimeStartUp <= 0)
            {
                if (StateOfGame == GameState.seek)
                {
                    TurnOffAllModelOfHideCharacter();
                }
                StartGame = true;
                UpdateTimePlay();
                // X? lý trong ván
                //if (HidePlayer != null)
                //{
                //    if (HidePlayer.IsImprisoned)
                //    {
                        
                        
                //        if(!WinGame)    LoseGameAction();
                //    }
                //}
                // X? lý khi k?t thúc ván
                if (!EndGame)
                {
                    if (TimePlay <= 0)
                    {
                        EndGame = true;
                        ResetStateForPlayer();
                        if (StateOfGame == GameState.hide)
                        {
                            if (!LoseGame)
                            {
                                WinGameAction();
                            }
                        }
                        else if (StateOfGame == GameState.seek)
                        {
                            if (SeekPlayer.CharacerInImprison > Character.Length / 2)
                            {
                                WinGameAction();
                            }
                            else if (!WinGame)
                            {
                                LoseGameAction();
                            }
                        }
                    }
                }
            }
        }
        else
        {
            TimePlay = 10f;
            TimeStartUp = 4f;
            StartGame = false; EndGame = false;
            WinGame = false; LoseGame = false;
        }
    }
    void UpdateTimePlay()
    {
        CountDownToStartUp.enabled = false;
        TimePlay = Mathf.Max(TimePlay - Time.deltaTime, 0);
        CountDownTime.text = Mathf.Round(TimePlay).ToString();
    }
    public void BeforeStartGame()
    {
        // Set state
        StartGame = false; EndGame = false;
        WinGame = false; LoseGame = false;
        onClick = false;
        //Camera
        Camera.GetComponent<CameraController>().ResetTransform();
        var SeekPlayer = Player.transform.GetChild(0);
        //SeekPlayer
        Player.transform.position = InitTransform.position;
        SeekPlayer.gameObject.SetActive(false);
        SeekPlayer.transform.localPosition = Vector3.zero;
        SeekPlayer.GetComponent<SeekStateManager>().ResetCharaceterInImprison();
        //HidePlayer
        Player.transform.GetChild(1).gameObject.SetActive(true);
        Player.transform.GetChild(1).GetComponent<HideStateManager>().ResetState();

        for (int j = 0; j < Character.Length; j++)
        {
            var SeekCharacter = Character[j].transform.GetChild(0);
            var HideCharacter = Character[j].transform.GetChild(1);
            //Seek
            SeekCharacter.gameObject.SetActive(false);
            SeekCharacter.transform.localPosition = Vector3.zero;
            SeekPlayer.GetComponent<SeekStateManager>().ResetCharaceterInImprison();    

            //Hide
            HideCharacter.gameObject.SetActive(true);
            HideCharacter.GetComponent<HideStateManager>().ResetState();
        }
    }
    public void OnSeekState()
    {
        if (!onClick)
        {
            StateOfGame = GameState.seek;
            onClick = true;
            joystick.SetActive(true);
            //HideBtn.SetActive(false);
            //SeekBtn.SetActive(false);
            MenuPanel.SetActive(false);
            InsSeek();
        }
    }
    public void OnHideState()
    {
        if (!onClick)
        {
            int randomIndex = Random.Range(0, 5);
            //HideBtn.SetActive(false);
            //SeekBtn.SetActive(false);
            MenuPanel.SetActive(false);
            StateOfGame = GameState.hide;
            onClick = true;
            joystick.SetActive(true);
            InsHide(randomIndex);
        }
    }
    public void InsHide(int i)
    {
        var SeekCharacter = Character[i].transform.GetChild(0);
        SeekEnemies = SeekCharacter.gameObject;
        var HideCharacter = Character[i].transform.GetChild(1);
        SeekCharacter.GetComponent<SeekStateManager>().CharacerInImprison = 0;
        SeekCharacter.gameObject.SetActive(true);
        HideCharacter.gameObject.SetActive(false);

    }
    public void InsSeek() 
    {
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void TurnOffAllModelOfHideCharacter()
    {
        foreach(GameObject i in Character)
        {
            i.transform.GetChild(1).GetComponent<HideStateManager>().TurnOffModel();
        }
    }
    public void WinGameAction()
    {
        ResetStateOfAllCharacerAndPlayer();
        WinPanel.SetActive(true);
        EndGame = true;
        WinGame = true;
        joystick.SetActive(false);
    }
    public void LoseGameAction()
    {
        ResetStateOfAllCharacerAndPlayer();
        LosePanel.SetActive(true);
        EndGame = true;
        LoseGame = true;
        joystick.SetActive(false);
    }
    public void ResetStateOfAllCharacerAndPlayer() {
        foreach (GameObject i in Character)
        {
            i.transform.GetChild(1).GetComponent<HideStateManager>().ResetState();
            i.transform.GetChild(0).GetComponent<SeekStateManager>().ResetState();
        }
        ResetStateForPlayer();
    }
    public void OnClickPlayAgain()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        MenuPanel.SetActive(true);
        BeforeStartGame();
    }
    public void OnClickNextLevel()
    {
        WinPanel.SetActive(false);
        MenuPanel.SetActive(true);
        BeforeStartGame();
        indexCurentLevel++;
        InsLevel(indexCurentLevel);
        
    }
    private void InsLevel(int index)
    {
        GameObject CurrentLevelOfGame = GameObject.Find("Level" + (index - 1).ToString());
        Destroy(CurrentLevelOfGame);
        string nameCurrentLevel = "Level" + index.ToString();
        GameObject currentLoadLevel = Resources.Load<GameObject>($"Level/{nameCurrentLevel}");
        GameObject currentLevel = Instantiate(currentLoadLevel);
    }
    private void ResetStateForPlayer()
    {
        if(StateOfGame == GameState.hide)
        {
            var MovementHidePlayer = Player.transform.GetChild(1).GetComponent<MovementPlayer>();
            MovementHidePlayer.MovementState(0);
            MovementHidePlayer.SetStateIdle();
        }   
        else if(StateOfGame == GameState.seek)
        {
            var MovementSeekPlayer = Player.transform.GetChild(0).GetComponent<MovementPlayer>();
            MovementSeekPlayer.MovementState(0);
            MovementSeekPlayer.SetStateIdle();
        }
        
    }
}
