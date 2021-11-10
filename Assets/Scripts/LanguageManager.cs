using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    /* instance of this singleton class */
    public static LanguageManager instance { get; private set; }

    /* abbreviation of selected language */
    private string chosenLang;

    /* tells us if language manager is already setted up or not */
    private bool isLanguageManagerSet = false;

    /* keys of Dictionary are names of elements in Unity, which text needs to be translated - values are exact text values which will be displayed on top of mentioned elements */
    private Dictionary<string, string> dictionary;

    /* whene scene is loaded */
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /* inits language manager - "CZ" or "EN" are currently possible options */
    public void setLanguageManager(string lang)
    {
        if (lang.Equals("EN") || lang.Equals("CZ"))
        {
            chosenLang = lang;
        }
        else /* invalid language selected, "ERR" will be used */
        {
            chosenLang = "ERR"; 
        }
        initDictionary();
    }

    /* inits Dictionary */ 
    private void initDictionary()
    {
        dictionary = new Dictionary<string, string>();
        if (chosenLang.Equals("EN"))
        {
            /* scene GameScene - score */
            dictionary.Add("ScoreLblNoChange", "Score: ");
            /* scene GameScene - main menu */
            dictionary.Add("GoMainMenuLbl", "Main menu");

            /* scene GameScene - GameOver */
            dictionary.Add("GameOverLbl", "GAME OVER");
            dictionary.Add("PlayAgainLbl", "Play again");
            
            /* scene GameScene - GamePause */
            dictionary.Add("GamePauseLbl", "GAME PAUSED");
            dictionary.Add("PauseLbl", "Continue playing");

            /* scene GameScene - Reconnect */
            dictionary.Add("ReconnectingLbl", "Mindwave disconnected\n" + "CONNECTING...");

            /* scene MainMenu - ExitMenu */
            dictionary.Add("ExitQuestionLbl", "REALLY QUIT?");
            dictionary.Add("NoAnswerLbl", "No, let´s go back");
            dictionary.Add("YesAnswerLbl", "Yes, quit");

            /* scene MainMenu - MainMenu */
            dictionary.Add("PlayLbl", "Play!");
            dictionary.Add("SettingsLbl", "Settings");
            dictionary.Add("ControlsLbl", "Controls");
            dictionary.Add("QuitLbl", "Quit");
            dictionary.Add("ScoreLbl", "Best score: ");

            /* scene MainMenu - SettingsMenu ('U' in name of element = upper) */
            dictionary.Add("LanguageULbl", "Language");
            dictionary.Add("SoundULbl", "Sound");
            dictionary.Add("ControlsULbl", "Use Mindwave");
            dictionary.Add("BackSLbl", "BACK");

            /* scene MainMenu - ControlsHelpMenu */
            dictionary.Add("ControlsText",
                "Goal of the game is to increase score by eating food (red apples).\n" +
                "When snake collides with its own body, player looses one of three healths and part of tail behind part of collision is eaten.\n" +
                "In order to regain health, player may pick regenerator (blue cube).\n" +
                "The snake starts to turn back, once is on border of map (too far from center).\n\n" +

                "Keyboard reacts to key-input all the time. Snake is controlled by arrows or WSAD.\n" +
                "After connection of Eye-tracker, snake starts to react on player's eyes.\n" +
                "It is possible to choose, whether to use NeuroSky Mindwave (headband) in Settings menu.\n" +
                "Without Mindwave, the snake is moving with constant speed and health is restored immediately after picking regenerator.\n" +
                "With Mindwave, player needs at least 30 % of concentration. The higher player attention is, the faster snake moves. After picking regenerator, " +
                "it is required to meditate for 5 seconds. Averge value of meditation has to be 50 % for successful health restore."
            );
            dictionary.Add("BackCLbl", "BACK");
        }
        else if (chosenLang.Equals("CZ"))
        {
            /* scene GameScene - score */
            dictionary.Add("ScoreLblNoChange", "Skóre: ");
            /* scene GameScene - main menu */
            dictionary.Add("GoMainMenuLbl", "Hlavní menu");

            /* scene GameScene - GameOver */
            dictionary.Add("GameOverLbl", "KONEC HRY");
            dictionary.Add("PlayAgainLbl", "Hrát znovu");

            /* scene GameScene - GamePause */
            dictionary.Add("GamePauseLbl", "HRA POZASTAVENA");
            dictionary.Add("PauseLbl", "Pokračovat ve hře");

            /* scene GameScene - Reconnect */
            dictionary.Add("ReconnectingLbl", "Mindwave odpojena\n" + "PŘIPOJUJI...");

            /* scene MainMenu - ExitMenu */
            dictionary.Add("ExitQuestionLbl", "OPRAVDU SKONČIT?");
            dictionary.Add("NoAnswerLbl", "Ne, vrátit se zpět");
            dictionary.Add("YesAnswerLbl", "Ano, ukončit hru");

            /* scene MainMenu - MainMenu */
            dictionary.Add("PlayLbl", "Hrát!");
            dictionary.Add("SettingsLbl", "Nastavení");
            dictionary.Add("ControlsLbl", "Ovládání");
            dictionary.Add("QuitLbl", "Ukončit hru");
            dictionary.Add("ScoreLbl", "Nejlepší skóre: ");

            /* scene MainMenu - SettingsMenu ('U' in name of object = upper) */
            dictionary.Add("LanguageULbl", "Jazyk");
            dictionary.Add("SoundULbl", "Zvuk");
            dictionary.Add("ControlsULbl", "Použít Mindwave");
            dictionary.Add("BackSLbl", "ZPĚT");

            /* scene MainMenu - ControlsHelpMenu */
            dictionary.Add("ControlsText",
                "Cíl hry je navyšovat skóre sbíráním jídla (červená jablka).\n" +
                "Po srážce s tělem hada, hráč ztrácí jeden ze tří životů a tato část je snězena až ke konci ocasu.\n" +
                "Pro obnovení života může hráč sebrat regenerátor (modrá kostka) doplňující život.\n" +
                "Jakmile se had dostane na kraj mapy (příliš se vzdálí od počátku), začne se otáčet zpátky.\n\n" +

                "Klávesnice funguje vždy. Had se ovládá šipkama nebo WSAD.\n" +
                "Po připojení Eye-trackeru had začne reagovat na ovládání očima.\n" +
                "V menu Nastavení je možné zvolit použítí Neurosky Mindwave (čelenka).\n" +
                "Bez Mindwave se had pohybuje konstantní rychlostí a život je doplněn okamžitě po sebrání regenerátoru.\n" +
                "S Mindwave se musí hráč soustředit alespoň na 30 %. Čím více se soustředí, tím rychleji se had pohybuje. \n" +
                "Po sebrání regenerátoru se medituje po dobu 5 sekund a průměrná hodnota meditace musí být 50 % pro úspěšnou obnovu života."
            );
            dictionary.Add("BackCLbl", "ZPĚT");
        }
    }

    /* method takes name of element and returns a string in selected language, which will be displayed on a top of the element */
    public string getTranslatedString(string sToTranslate)
    {
        if (dictionary.ContainsKey(sToTranslate))
        {
            return dictionary[sToTranslate];
        }
        else
        {
            return "ERR"; /* Dictionary is not able to translate the element (= not present) */
        }
    }

    /* getter for selected language */
    public string getActiveLanguage()
    {
        return chosenLang;
    }
    
    /* sets a flag, which tells us if language manager is already setted up or not */
    public void setLanguageManagerSetted(bool value)
    {
        isLanguageManagerSet = value;
    }

    /* returns a flag, which tells us if language manager is already setted up or not */
    public bool getLanguageManagerSetted()
    {
        return isLanguageManagerSet;
    }
}
