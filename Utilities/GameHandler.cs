using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : Singleton<GameHandler>
{
    public GameData GameData;
    public Cheats Cheats;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static void UpdateCheat(string key, bool value, string param)
    {
        CheatCodes code = (CheatCodes)System.Enum.Parse(typeof(CheatCodes), key);

        Instance.Cheats.ActiveCode(code, value, param);
    }
    public static void UpdateCheat(CheatCodes code, bool value, string param)
    {
        Instance.Cheats.ActiveCode(code, value, param);
    }

    public static bool IsCheatActive(CheatCodes code)
    {
        return Instance.Cheats.IsCheatActive(code);
    }

    public static T ParseCheatParam<T>(CheatCodes code)
    {
        return Instance.Cheats.ParseCheatParam<T>(code);
    }

    public static string CheatParam(CheatCodes code)
    {
        return Instance.Cheats.CheatParam(code);
    }


}
