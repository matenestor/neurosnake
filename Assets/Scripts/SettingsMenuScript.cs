using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    /* reference to flag images (represents language) */
    public GameObject[] languageFlags;

    /* reference to images, which are used to represent control options */
    public Toggle UseMindwaveToogle;

    /* reference to buttons, which represent the state of sound (on / off) */
    public GameObject soundOffBtn;
    public GameObject soundOnBtn;

    /* reference to icons, which represent the state of sound (on / off) */
    public GameObject soundOffIcon;
    public GameObject soundOnIcon;

    /* currently displayed flag (represents current language) */
    private GameObject activeFlag = null;

    /* after scene load */
    void Awake()
    {
        setGraphics();
    }

    /* sets proper graphics (image of flag, sound, etc.) according to information in settings.dat */
    public void setGraphics()
    {
        /* find flag which belongs to chosen language */
        if (LanguageManager.instance.getActiveLanguage().Equals("CZ"))
        {
            for (int i = 0; i < languageFlags.Length; i++)
            {
                if (languageFlags[i].name.Equals("CzechLangIcon"))
                {
                    languageFlags[i].SetActive(true);
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < languageFlags.Length; i++)
            {
                if (languageFlags[i].name.Equals("EnglishLangIcon"))
                {
                    languageFlags[i].SetActive(true);
                    break;
                }
            }
        }

        bool? loadedMindwaveState = LoaderManager.instance.useMindwave;
        if(loadedMindwaveState != null) /* mindwave checkbox data successfully loaded from file */
        {
            if(loadedMindwaveState == true)
            {
                UseMindwaveToogle.isOn = true;
            }
            else
            {
                UseMindwaveToogle.isOn = false;
            }
        }
        else /* not loaded - use default */
        {
            UseMindwaveToogle.isOn = true;
        }
        // SOUND ICON IS TURNED OFF IN UNITY INSPECTOR, IF YOU WANT TO IMPLEMENT SOUND, 
        // TURN IT ON, IN ORDER TO SEE IT
        /* set sound icon */
        //bool? loadedSound = LoaderManager.instance.getLoadedSound();
        //if (loadedSound != null) /* sound data successfully loaded */
        //{
        //    if (loadedSound == true)
        //    {
        //        soundOnBtn.SetActive(true);
        //    }
        //    else
        //    {
        //        soundOffBtn.SetActive(true);
        //    }
        //}
        //else /* sound data not loaded - default state = sound on */
        //{
        //    soundOnBtn.SetActive(true);
        //}
    }

    public void useMindwaveTogglePressed() {
        // invert bool value of using Mindwave
        LoaderManager.instance.setUseMindwave(UseMindwaveToogle.isOn);
        
        // if check button pressed to off
        if (!UseMindwaveToogle.isOn) {
            // if Mindwave is still connected or connecting -- disconnect
            if (MindwaveManager.Instance.Controller.IsConnected
                    || MindwaveManager.Instance.Controller.IsConnecting) {

                MindwaveManager.Instance.Controller.Disconnect();
            }
        }
    }

    /* action, which will be performed after click on arrow pointing to left (language) */
    public void leftArrowLangPressed()
    {
        /* we will determine currently displayed flag and then we change it to previous flag in the list */
        for(int i = 0; i < languageFlags.Length; i++)
        {
            if (languageFlags[i].activeSelf)
            {
                int targetIndex = i - 1;
                if(targetIndex < 0)/* if we are out of bounds, then we move to the end of the list */
                {
                    languageFlags[languageFlags.Length - 1].SetActive(true);
                    activeFlag = languageFlags[languageFlags.Length - 1];
                }
                else
                {
                    languageFlags[targetIndex].SetActive(true);
                    activeFlag = languageFlags[targetIndex];
                }
                languageFlags[i].SetActive(false);
                changeLanguageUI();
                break;
            }
        }
    }

    /* action, which will be performed after click on arrow pointing to right (language) */
    public void rightArrowLangPressed()
    {
        /* we will determine currently displayed flag and then we change it to next flag in the list */
        for (int i = 0; i < languageFlags.Length; i++)
        {
            if (languageFlags[i].activeSelf)
            {
                int targetIndex = i + 1;
                if (targetIndex > (languageFlags.Length - 1))/* if we are out of bounds, then we move to the beginning of the list */
                {
                    languageFlags[0].SetActive(true);
                    activeFlag = languageFlags[0];
                }
                else
                {
                    languageFlags[targetIndex].SetActive(true);
                    activeFlag = languageFlags[targetIndex];
                }
                languageFlags[i].SetActive(false);
                changeLanguageUI();
                break;
            }
        }
    }

    /* updates GUI language after change of language in settings */
    public void changeLanguageUI()
    {
        if (activeFlag != null)
        {
            switch (activeFlag.name)
            {
                case "CzechLangIcon":
                    LanguageManager.instance.setLanguageManager("CZ");
                    LoaderManager.instance.setLoadedLang("CZ");
                    break;

                case "EnglishLangIcon":
                    LanguageManager.instance.setLanguageManager("EN");
                    LoaderManager.instance.setLoadedLang("EN");
                    break;
            }
        }
    }
    
    /* after click on button which turns sound off */
    public void soundOffIconPressed()
    {
        soundOffBtn.SetActive(false);
        soundOffIcon.SetActive(false);
        soundOnBtn.SetActive(true);
        soundOnIcon.SetActive(true);
    }

    /* after click on button which turns sound on */
    public void soundOnIconPressed()
    {
        soundOnBtn.SetActive(false);
        soundOnIcon.SetActive(false);
        soundOffBtn.SetActive(true);
        soundOffIcon.SetActive(true);
    }

    /* saves settings */
    public void saveSettings()
    {
        string selectedLang = null;
        long highScore = -1;

        /* we will determine the currently display flag and change it to previous in the list */
        for (int i = 0; i < languageFlags.Length; i++)
        {
            if (languageFlags[i].activeSelf)
            {
                if (languageFlags[i].name.Equals("CzechLangIcon"))
                {
                    selectedLang = "CZ";
                }else if (languageFlags[i].name.Equals("EnglishLangIcon"))
                {
                    selectedLang = "EN";
                }
                    break;
            }
        }
        
        highScore = LoaderManager.instance.getLoadedBestScore();
        LoaderManager.instance.SaveSettings(selectedLang, UseMindwaveToogle.isOn, highScore);
    }
}
