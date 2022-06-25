using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Variable
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
    public GameObject GamePlayPanel;
    public GameObject FunctionPanel;

    private float TimePlay, TimeStartUp;
    [Header("Text")]
    public TextMeshProUGUI CountDownTime;
    public Text CountDownToStartUp;

    [Header("Core")]
    public GameObject joystick;
    public GameObject Camera;
    public Image Fill;
    public Gradient gradient;
    public Slider slider;

    [Header("Button")]
    public GameObject HideBtn;
    public GameObject SeekBtn;
    
    [HideInInspector]
    public int indexCurentLevel;
    public TextMeshProUGUI showLevelTxt;
    private Transform InitTransform;

    #endregion

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        indexCurentLevel = 1;
        showLevelTxt.text = indexCurentLevel.ToString();
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

            // Time
            CountDownToStartUp.enabled = true;
            TimeStartUp -= Time.deltaTime;
            CountDownToStartUp.text = Mathf.Round(TimeStartUp).ToString();
            
            //StartGame
            if(TimeStartUp <= 0)
            {
                if (StateOfGame == GameState.seek)
                {
                    TurnOffAllModelOfHideCharacter();
                }
                StartGame = true;
                UpdateTimePlay();
                #region Process Endgame
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
                #endregion
            }
        }
        else
        {
            TimePlay = 20f;
            SetMaxTimePlay(TimePlay);
            TimeStartUp = 4f;
            StartGame = false; EndGame = false;
            WinGame = false; LoseGame = false;
        }
    }
    
    

    #region Mode Game Play
    public void OnSeekState()
    {
        if (!onClick)
        {
            StateOfGame = GameState.seek;
            onClick = true;
            joystick.SetActive(true);
            MenuPanel.SetActive(false);
            GamePlayPanel.SetActive(true);
            InsSeek();
        }
    }
    public void OnHideState()
    {
        if (!onClick)
        {
            int randomIndex = Random.Range(0, 5);
            MenuPanel.SetActive(false);
            GamePlayPanel.SetActive(true);
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
    #endregion
    #region Core Panel
    public void WinGameAction()
    {
        if (!LoseGame)
        {
            ResetStateOfAllCharacerAndPlayer();
            WinPanel.SetActive(true);
            GamePlayPanel.SetActive(false);
            FunctionPanel.SetActive(false);
            EndGame = true;
            WinGame = true;
            joystick.SetActive(false);

        }
    }
    public void LoseGameAction()
    {
        if (!WinGame)
        {
            ResetStateOfAllCharacerAndPlayer();
            LosePanel.SetActive(true);
            GamePlayPanel.SetActive(false);
            FunctionPanel.SetActive(false);
            EndGame = true;
            LoseGame = true;
            joystick.SetActive(false);
        }
    }
    #endregion
    #region ButtonClick
    public void OnClickPlayAgain()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        MenuPanel.SetActive(true);
        FunctionPanel.SetActive(true);
        CountDownTime.text = "00";
        BeforeStartGame();
    }
    public void OnClickNextLevel()
    {
        CountDownTime.text = "00";
        WinPanel.SetActive(false);
        MenuPanel.SetActive(true);
        FunctionPanel.SetActive(true);
        BeforeStartGame();
        indexCurentLevel++;
        showLevelTxt.text = indexCurentLevel.ToString();
        InsLevel(indexCurentLevel);

    }
    #endregion
    #region Core Funcion

    public void SetMaxTimePlay(float time)
    {
        slider.maxValue = time;
        slider.value = time;
        Fill.color = gradient.Evaluate(1f);
    }
    public void SetCurrentTime(float time)
    {
        slider.value = time;
        Fill.color = gradient.Evaluate(slider.normalizedValue);
    }
    private void UpdateTimePlay()
    {

        CountDownToStartUp.enabled = false;
        TimePlay = Mathf.Max(TimePlay - Time.deltaTime, 0);
        SetCurrentTime(TimePlay);
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
        var HidePlayer = Player.transform.GetChild(1);
        //SeekPlayer
        Player.transform.position = InitTransform.position;
        SeekPlayer.gameObject.SetActive(false);
        SeekPlayer.transform.localPosition = Vector3.zero;
        SeekPlayer.GetComponent<SeekStateManager>().ResetCharaceterInImprison();
        //HidePlayer
        Player.transform.GetChild(1).gameObject.SetActive(true);
        Player.transform.GetChild(1).GetComponent<HideStateManager>().ResetState();

        //Reset Transform
        SeekPlayer.GetComponent<MovementPlayer>().ResetPositionPlayer();
        HidePlayer.GetComponent<MovementPlayer>().ResetPositionPlayer();
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
    public void ResetStateOfAllCharacerAndPlayer()
    {
        foreach (GameObject i in Character)
        {
            i.transform.GetChild(1).GetComponent<HideStateManager>().ResetState();
            i.transform.GetChild(0).GetComponent<SeekStateManager>().ResetState();
        }
        ResetStateForPlayer();
    }
    public void TurnOffAllModelOfHideCharacter()
    {
        foreach (GameObject i in Character)
        {
            i.transform.GetChild(1).GetComponent<HideStateManager>().TurnOffModel();
        }
    }
    #endregion
}
