using System.IO;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System;

public class LoaderManager : MonoBehaviour
{
    /* instance of this singleton class */
    public static LoaderManager instance { get; private set; }

    /* flag which tells the state of settings load (true if we already tried to load settings from file - else false) */
    private bool triedSettingsLoad = false;

    /* variables which hold the settings values */
    private string loadedLang = null;
    private long loadedBestScore = -1;

    /* if Mindwave will be used or not (set in SettingsMenuScript.cs, used in MovementSnake.cs) */
    public bool? useMindwave;

    /* used for settings file crypto */
    private string hashCode = "zio7dc5@e";

    /* when scene is loaded */
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /* saves settings to file
     * we need to save: language, mindwave checkbox state + top score */
    public void SaveSettings(string lang, bool? mindwave, long bestScore)
    {
        string finalString = lang + " " + mindwave + " " + bestScore;
        string file = "settings.dat";
        if (File.Exists(file)) /* delete existing settings file */
        {
            File.Delete(file);
        }

        byte[] data = UTF8Encoding.UTF8.GetBytes(finalString);
        using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
        {
            byte[] keys = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashCode));
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider()
            {
                Key = keys,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                ICryptoTransform cryptoTrans = tripleDES.CreateEncryptor();
                byte[] result = cryptoTrans.TransformFinalBlock(data, 0, data.Length);
                using (StreamWriter sw = File.CreateText(file))
                {
                    sw.WriteLine(Convert.ToBase64String(result, 0, result.Length));
                }
            }
        }
    }

    /* load settings from file */
    public void LoadSettings()
    {
        triedSettingsLoad = true;

        string file = "settings.dat";
        if (!File.Exists(file)) /* if file with settings doesn´t exist, then return */
        {
            return;
        }
        /* load first line of file with settings */
        string textFileEncrypt = File.ReadAllLines(file).First();
        byte[] data = Convert.FromBase64String("");

        try
        {
            data = Convert.FromBase64String(textFileEncrypt);
        }catch(Exception e) //error read file - probably modified
        {
            if (File.Exists(file)) /* delete existing settings file */
            {
                File.Delete(file);
            }
            return;
        }

        using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
        {
            byte[] keys = hash.ComputeHash(UTF8Encoding.UTF8.GetBytes(hashCode));
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider()
            {
                Key = keys,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            })
            {
                    ICryptoTransform cryptoTrans = tripleDES.CreateDecryptor();

                    byte[] result = cryptoTrans.TransformFinalBlock(data, 0, data.Length);
                    string textFileDecrypt = UTF8Encoding.UTF8.GetString(result);

                    string[] splitDecrypt = textFileDecrypt.Split(' ');
                    if (splitDecrypt.Length == 3)/* if size is != 3, then the settings file is probably damaged */
                    {
                        /* quick check if settings.dat contains valid values */
                        if (splitDecrypt[0].Equals("EN") || splitDecrypt[0].Equals("CZ"))
                        {
                            loadedLang = splitDecrypt[0];
                        }

                        if (splitDecrypt[1].Equals("True"))
                        {
                            useMindwave = true;
                        }
                        else if (splitDecrypt[1].Equals("False"))
                        {
                            useMindwave = false;
                        }

                        long.TryParse(splitDecrypt[2], out loadedBestScore);/* if an error occurs, then -1 */
                    }           
            }
        }
    }

    /* getter for loadedLang - if language is not loaded, then value is null */
    public string getLoadedLang()
    {
        return loadedLang;
    }

 
    /* getter for useMindwave - if mindwave is used, return true else return false */
    public bool? getUseMindwave()
    {
        return useMindwave;
    }

    /* getter for loadedBestScore - if best score is not loaded, then value is -1 */
    public long getLoadedBestScore()
    {
        return loadedBestScore;
    }

    public bool getTriedSettingsLoad()
    {
        return triedSettingsLoad;
    }

    /* used for changing language after load (the language can be different from settings.dat) */
    public void setLoadedLang(string lang)
    {
        this.loadedLang = lang;
    }

    /* setter for useMindwave */
    public void setUseMindwave(bool? state)
    {
        this.useMindwave = state;
    }

    /* used for changing best score value after load (value can be different from settings.dat) */
    public void setBestScore(long score)
    {
        this.loadedBestScore = score;
    }
}
