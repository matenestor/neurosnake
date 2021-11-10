using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneScript : MonoBehaviour
{
    /* reference to objects included in game scene */
    /* score */
    public GameObject ScoreLblNoChange;
    public GameObject ActualScoreLblGS;

    /* game over */
    public GameObject GameOverLblGS;
    public GameObject PlayAgainLblGS;
    public GameObject GoMainMenuLblGS;

    /* game paused */
    public GameObject GamePauseLblGS;
    public GameObject PlayContinueLblGS;
    public GameObject PlayAgainLblGSPause;

    /* hearts */
    public GameObject FirstHeartIconGS;
    public GameObject SecondHeartIconGS;
    public GameObject ThirdHeartIconGS;

    /* indicates brain activity */
    public GameObject MeditationActivitySliderGS;
    public GameObject BrainActivitySliderGS;

    /* window to show while reconnecting */
    public GameObject Reconnect;
    /* window to show on game-over */
    public GameObject GameOver;
    /* window to show on game pause */
    public GameObject GamePause;

    /* mindwave disconnected */
    public GameObject ReconnectingLbl;

    private Slider MeditationActivitySliderGSInit;
    private Slider BrainActivitySliderGSInit;
    private TextMeshProUGUI ActualScoreLblGSInit;

    /* reference to MovementSnake.cs script */
    private MovementSnake script_movementSnake;

    /* count of hearts displayed in scene */
    private int availableHearts;
    /* actual score */
    private int score;

    private void Start()
    {
        availableHearts = 3;
        score = 0;

        // set score to 0 in GUI
        ActualScoreLblGSInit.text = Convert.ToString(score);
    }

    /* when scene is loaded */
    void Awake()
    {
        /* if translate of GUI is needed = language is not set to English */
        if (!LanguageManager.instance.getActiveLanguage().Equals("EN"))
        {
            translateGameSceneScene();
        }

        /* get reference to brain activity Sliders */
        MeditationActivitySliderGSInit = MeditationActivitySliderGS.GetComponent<Slider>();
        BrainActivitySliderGSInit = BrainActivitySliderGS.GetComponent<Slider>();

        /* get reference to score label */
        ActualScoreLblGSInit = ActualScoreLblGS.GetComponent<TextMeshProUGUI>();

        // get GameSceneScript.cs script
        script_movementSnake = GameObject.Find("/Snake/HeadPref").gameObject.GetComponent<MovementSnake>();
    }

    /* translate GUI components of scene GameScene to selected lanuguage */
    public void translateGameSceneScene()
    {
        TextMeshProUGUI GameOverLbltxt      = GameOverLblGS.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PlayAgainLbltxt     = PlayAgainLblGS.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI GamePauseLbltxt     = GamePauseLblGS.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PauseLbltxt         = PlayContinueLblGS.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI GoMainMenuLbltxt    = GoMainMenuLblGS.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ScoreLblNoChangetxt = ScoreLblNoChange.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI PlayAgainLblGSPausetxt = PlayAgainLblGSPause.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ReconnectingLbltxt = ReconnectingLbl.GetComponent<TextMeshProUGUI>();

        GameOverLbltxt.text      = LanguageManager.instance.getTranslatedString("GameOverLbl");
        PlayAgainLbltxt.text     = LanguageManager.instance.getTranslatedString("PlayAgainLbl");
        GamePauseLbltxt.text     = LanguageManager.instance.getTranslatedString("GamePauseLbl");
        PauseLbltxt.text         = LanguageManager.instance.getTranslatedString("PauseLbl");
        GoMainMenuLbltxt.text    = LanguageManager.instance.getTranslatedString("GoMainMenuLbl");
        ScoreLblNoChangetxt.text = LanguageManager.instance.getTranslatedString("ScoreLblNoChange");
        PlayAgainLblGSPausetxt.text = LanguageManager.instance.getTranslatedString("GoMainMenuLbl");
        ReconnectingLbltxt.text = LanguageManager.instance.getTranslatedString("ReconnectingLbl");
    }

    /* action when main menu button is pressed */
    public void GoMainMenuBtnPressed()
    {
        SceneManager.LoadScene("MainMenu");

        if (script_movementSnake.useMindwave) {
            MindwaveManager.Instance.Controller.OnUpdateMindwaveData -= script_movementSnake.OnUpdateMindwaveData;
            MindwaveManager.Instance.Controller.OnConnectMindwave    -= script_movementSnake.OnConnectMindwave;
            MindwaveManager.Instance.Controller.OnDisconnectMindwave -= script_movementSnake.OnDisconnectMindwave;
            MindwaveManager.Instance.Controller.OnConnectionTimeout  -= script_movementSnake.OnConnectionTimeout;
        }
    }

    /* show while connecting to Mindwave */
    public void ShowReconnectWindow(bool status) {
        Reconnect.SetActive(status);
    }

    /* when snake dies */
    public void ShowGameOverWindow() {
        GameOver.SetActive(true);
        saveScore();
    }

     /* if new best score, then save it */
    public void saveScore()
    {
        long bestScore = LoaderManager.instance.getLoadedBestScore();

        if(this.score > bestScore)
        {
            LoaderManager.instance.setBestScore(this.score);
            LoaderManager.instance.SaveSettings(LoaderManager.instance.getLoadedLang(), LoaderManager.instance.getUseMindwave(), LoaderManager.instance.getLoadedBestScore());
        }
    }

    /* action when play again button is pressed */
    public void PlayAgainBtnPressed() {
        // reset HP and score
        resetHearts();
        resetScore();
        // reset position of snake
        script_movementSnake.SnakeReset();
        // hide GameOver window
        GameOver.SetActive(false);
    }

    /* action when play continue button is pressed */
    public void PauseBtnPressed() {
        // change bool value on ESC/button pressed
        script_movementSnake.isPaused = !script_movementSnake.isPaused;

        // show/hide GamePause window
        GamePause.SetActive(script_movementSnake.isPaused);
    }

    /* --- METHODS WHICH ARE NOT CONNECTED WITH TRANSLATION BEGIN --- */

    /* sets value of meditation activity indicator, which is located in the game scene
     * value can be selected from <0, 100>
     */
    public void setMeditationActivitySliderValue(int value)
    {
        MeditationActivitySliderGSInit.value = value;
    }

    /* sets value of brain activity indicator, which is located in the game scene
     * value can be selected from <0, 100>
     */
    public void setBrainActivitySliderValue(int value)
    {
        BrainActivitySliderGSInit.value = value;
    }

    /* removes a heart which represents life from scene - if there are any... */
    public void removeHeart()
    {
        switch (availableHearts)
        {
            case 3:
                ThirdHeartIconGS.SetActive(false);
                availableHearts--;
                break;

            case 2:
                SecondHeartIconGS.SetActive(false);
                availableHearts--;
                break;

            case 1:
                FirstHeartIconGS.SetActive(false);
                availableHearts--;
                break;

            case 0:
                availableHearts--;
                break;
        }
    }

    /* add a heart which represents life from scene - if there are any... */
    public void addHeart() {
        switch (availableHearts) {
            case 2:
                ThirdHeartIconGS.SetActive(true);
                availableHearts++;
                break;

            case 1:
                SecondHeartIconGS.SetActive(true);
                availableHearts++;
                break;

            case 0:
                FirstHeartIconGS.SetActive(true);
                availableHearts++;
                break;
        }
    }

    /* reset heart count */
    public void resetHearts()
    {
        availableHearts = 3;

        FirstHeartIconGS.SetActive(true);
        SecondHeartIconGS.SetActive(true);
        ThirdHeartIconGS.SetActive(true);
    }

    /* reset score */
    public void resetScore() {
        score = 0;
        ActualScoreLblGSInit.text = Convert.ToString(score);
    }

    /* returns number of available hearts */
    public int getAvailableHearts()
    {
        return availableHearts;
    }

    /* adds a number to actual score (can be also used to remove some number) */
    public void addToScore(int scoreToAdd)
    {
        /* score cannot be negative */
        if (score + scoreToAdd >= 0) {
            ActualScoreLblGSInit.text = Convert.ToString(score + scoreToAdd);
            score += scoreToAdd;
        }
    }

    /* returns current score */
    public int getActualScore()
    {
        return score;
    }
    /* --- METHODS WHICH ARE NOT CONNECTED WITH TRANSLATION END --- */
}
