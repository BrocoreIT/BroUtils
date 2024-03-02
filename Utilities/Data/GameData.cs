using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="GameData", menuName ="Data/GameData")]
public class GameData : ScriptableObject
{

    public bool isGamePlaying;

    public bool isDebug;
    [SerializeField]
    private bool hasLight;

    public DataBank<int, BroCurve> levelCurves;

    public bool isRestart;

    public bool HasLight { get => hasLight; set => hasLight = value; }
}
[System.Serializable]
public struct ColorData
{
    public Color lightColor;
    public Color objColor;
}


[System.Serializable]
public class Cheats
{
    public CustomList<CheatCodes, CheatCode> CheatChodes;

    public string CheatParam(CheatCodes code)
    {
        return CheatChodes.Get(code).parameter;
    }
    public T ParseCheatParam<T>(CheatCodes code)
    {
        return (T)System.Enum.Parse(typeof(T), CheatParam(code)) ;
    }
    public bool IsCheatActive(CheatCodes code)
    {
        CheatCode cheatCode = CheatChodes.Get(code);
        bool isOn = cheatCode.isOn;
      
        return isOn;
    }
    public void ActiveCode(CheatCodes cheat, bool value)
    {
        CheatCode cheatCode = CheatChodes.Get(cheat);

        ActiveCode(cheat, cheatCode, value, cheatCode.CheatToggle.Param);
    }

    public void ActiveCode(CheatCodes cheat, bool value, string param = "")
    {
        CheatCode cheatCode = CheatChodes.Get(cheat);

        ActiveCode(cheat, cheatCode, value,param);
    }

    private void ActiveCode(CheatCodes cheat, CheatCode cheatCode, bool value, string param = "")
    {
    

        if (cheatCode.isOn != value)
        {
            cheatCode.isOn = value;
            cheatCode.OnStatusChanged?.Invoke(value);
        }
        if (cheatCode.parameter != param)
        {
            cheatCode.parameter = param;
            cheatCode.OnValueParamChanged?.Invoke(value, param);
        }
        cheatCode.OnValue?.Invoke(cheatCode.isOn, cheatCode.parameter);
        Log(cheat, value, param);
    }

    public void Log(CheatCodes cheat, bool status, string value)
    {
        BroUtils.Log(cheat.ToString(), $"satus : {status} \n Value : {value}");
    }
}

[System.Serializable]
public class CheatCode
{

    public bool isOn;
    public string parameter;
    public UnityEngine.Events.UnityEvent<bool, string> OnValue;
    public UnityEngine.Events.UnityEvent<bool> OnStatusChanged;
    public UnityEngine.Events.UnityEvent<bool,string> OnValueParamChanged;

    public CheatToggle CheatToggle;
}

public enum CheatCodes
{
    none,
    InfiniteGame,
    SelectName,
    SelectRegion,
    SelectCard
}