using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    /* reference to objects, which label needs to be translated MM = MainMenu, SM = SettingsMenu, CHM = ControlsHelpMenu, EM = ExitMenu + GS = GameScene */
    public GameObject PlayBtnMM;
    public GameObject SettingsBtnMM;
    public GameObject ControlsBtnMM;
    public GameObject QuitBtnMM;
    public GameObject ScoreLblMM;

    public GameObject LanguageULblSM;
    public GameObject SoundULblSM;
    public GameObject ControlsULblSM;
    public GameObject BackSBtnSM;

    public GameObject ControlsTextCHM;
    public GameObject BackCBtnCHM;

    public GameObject ExitQuestionLblEM;
    public GameObject NoAnswerLblEM;
    public GameObject YesAnswerLblEM;

    private void Start()
    {
        initScene();
    }

     /* performs basic steps needed for right scene display, ie.:
      * - set LanguageManager
      * - translate GUI components (if needed, ie.: different language than english is set)
      */
    public void initScene()
    {
        /* if settings not loaded */
        if (!LoaderManager.instance.getTriedSettingsLoad())
        {
            LoaderManager.instance.LoadSettings();
        }
        string loadedLang = LoaderManager.instance.getLoadedLang();

        /* if previously selected language was successfully loaded */
        if (loadedLang != null)
        {
            /* set LanguageManager */
            if (!LanguageManager.instance.getLanguageManagerSetted())
            {
                LanguageManager.instance.setLanguageManager(loadedLang);
                LanguageManager.instance.setLanguageManagerSetted(true);
            }

            if (!loadedLang.Equals("EN"))
            {
                /* translation of GUI is needed */
                translateMainMenuScene();
            }
        }
        else
        {/* if language is not set in configuration file settings.dat */
            if (!LanguageManager.instance.getLanguageManagerSetted())
            {
                if (Application.systemLanguage == SystemLanguage.Czech)
                {
                    LanguageManager.instance.setLanguageManager("CZ");
                }
                else /* in case of unknown language init LanguageManager to English */
                {
                    LanguageManager.instance.setLanguageManager("EN");
                }
                LanguageManager.instance.setLanguageManagerSetted(true);
            }

            /* if translation of GUI is needed, ie.: different language than english is set */
            if (Application.systemLanguage == SystemLanguage.Czech)
            {
                translateMainMenuScene();
            }
        }

        /* load high score */
        long bestScore = LoaderManager.instance.getLoadedBestScore();

        if (bestScore > 0)
        {
            setTopScore(bestScore);
        }
    }

    /* display high score loaded from file */
    public void setTopScore(long topScore)
    {
        ScoreLblMM.SetActive(true);
        TextMeshProUGUI ScoreLblMMtxt = ScoreLblMM.GetComponent<TextMeshProUGUI>();
        ScoreLblMMtxt.text = LanguageManager.instance.getTranslatedString("ScoreLbl") + topScore;
    }

    /* action performed when playBtn is pressed */
    public void playBtnPressed()
    {
        SceneManager.LoadScene("PlaySnake", LoadSceneMode.Single);
    }

    /* action performed when finalQuitBtn is pressed */
    public void finalQuitBtnPressed()
    {
        Application.Quit();
    }

    /* translate GUI components of scene MainMenu to selected language */
    public void translateMainMenuScene()
    {
        TextMeshProUGUI PlayBtnMMtxt = PlayBtnMM.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI SettingsBtnMMtxt = SettingsBtnMM.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI ControlsBtnMMtxt = ControlsBtnMM.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI QuitBtnMMtxt = QuitBtnMM.GetComponentInChildren<TextMeshProUGUI>();

        TextMeshProUGUI LanguageULblSMtxt = LanguageULblSM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI SoundULblSMtxt = SoundULblSM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ControlsULblSMtxt = ControlsULblSM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI BackSBtnSMtxt = BackSBtnSM.GetComponentInChildren<TextMeshProUGUI>();

        TextMeshProUGUI ControlsTextCHMtxt = ControlsTextCHM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI BackCBtnCHMtxt = BackCBtnCHM.GetComponentInChildren<TextMeshProUGUI>();

        TextMeshProUGUI ExitQuestionLblEMtxt = ExitQuestionLblEM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI NoAnswerLblEMtxt = NoAnswerLblEM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI YesAnswerLblEMtxt = YesAnswerLblEM.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ScoreLblMMtxt = ScoreLblMM.GetComponent<TextMeshProUGUI>();

        PlayBtnMMtxt.text = LanguageManager.instance.getTranslatedString("PlayLbl");
        SettingsBtnMMtxt.text = LanguageManager.instance.getTranslatedString("SettingsLbl");
        ControlsBtnMMtxt.text = LanguageManager.instance.getTranslatedString("ControlsLbl");
        QuitBtnMMtxt.text = LanguageManager.instance.getTranslatedString("QuitLbl");

        LanguageULblSMtxt.text = LanguageManager.instance.getTranslatedString("LanguageULbl");
        SoundULblSMtxt.text = LanguageManager.instance.getTranslatedString("SoundULbl");
        ControlsULblSMtxt.text = LanguageManager.instance.getTranslatedString("ControlsULbl");
        BackSBtnSMtxt.text = LanguageManager.instance.getTranslatedString("BackSLbl");

        ControlsTextCHMtxt.text = LanguageManager.instance.getTranslatedString("ControlsText");
        BackCBtnCHMtxt.text = LanguageManager.instance.getTranslatedString("BackCLbl");

        ExitQuestionLblEMtxt.text = LanguageManager.instance.getTranslatedString("ExitQuestionLbl");
        NoAnswerLblEMtxt.text = LanguageManager.instance.getTranslatedString("NoAnswerLbl");
        YesAnswerLblEMtxt.text = LanguageManager.instance.getTranslatedString("YesAnswerLbl");
        ScoreLblMMtxt.text = LanguageManager.instance.getTranslatedString("ScoreLbl");
        
        /* load and set high score */
        long bestScore = LoaderManager.instance.getLoadedBestScore();
        if (bestScore > 0)
        {
            setTopScore(bestScore);
        }
    }
}
