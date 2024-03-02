using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandlerBase<T> : BroSingleton<T> where T : Component
{
    public GameData GameData;
    public Cheats Cheats;
  

    public void UpdateCheat(string key, bool value, string param)
    {
        CheatCodes code = (CheatCodes)System.Enum.Parse(typeof(CheatCodes), key);

        Cheats.ActiveCode(code, value, param);
    }
    public void UpdateCheat(CheatCodes code, bool value, string param)
    {
        Cheats.ActiveCode(code, value, param);
    }

    public bool IsCheatActive(CheatCodes code)
    {
        return Cheats.IsCheatActive(code);
    }

    public K ParseCheatParam<K>(CheatCodes code)
    {
        return Cheats.ParseCheatParam<K>(code);
    }

    public string CheatParam(CheatCodes code)
    {
        return Cheats.CheatParam(code);
    }


}
