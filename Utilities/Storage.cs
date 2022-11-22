using UnityEngine;

/// <summary>
/// Helper for player prefs
/// </summary>
public static class Storage
{
    /*   public static void SetBool(string key,bool value)
       {
           PlayerPrefs.SetString(key, value.ToString());
           PlayerPrefs.Save();
       }*/
    public static bool SetBool(string key, bool value)
    {
        SetString(key, value.ToString());
        PlayerPrefs.Save();
        return value;
    }
    public static bool GetBool(string key, bool defaultValue = false)
    {
        return bool.Parse(GetString(key, defaultValue.ToString()));
    }

    public static float SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
        return value;
    }

    public static float GetFloat(string key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public static int SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
        return value;
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    public static string SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
        return value;
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

}
