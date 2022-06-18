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
        PlayerPrefs.SetString(key, value.ToString());
        PlayerPrefs.Save();
        return value;
    }
    public static bool GetBool(string key, bool defaultValue)
    {
        return bool.Parse(PlayerPrefs.GetString(key, defaultValue.ToString()));
    }
}
