using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CheatToggle : MonoBehaviour
{
    public CheatCodes CheatCodes;
    [SerializeField]
    private string param;
    public TMPro.TMP_Dropdown dropdown;
    public UnityEngine.UI.Toggle Toggle;
    public string Param { get => param; set => param = value; }
    private void OnEnable()
    {
        dropdown.ClearOptions();
        switch (CheatCodes)
        {
            case CheatCodes.SelectName:
              
                break;
            case CheatCodes.SelectRegion:
               
                break;
            case CheatCodes.SelectCard:
              /*  var options3 = new List<TMPro.TMP_Dropdown.OptionData>();
                Type enumType3 = typeof(BingoGames);
                SetEnumToDropdown<BingoGames>(options3, enumType3);*/
                break;
        }
       
    }

    private void SetEnumToDropdown<T>(List<TMP_Dropdown.OptionData> options3, Type enumType)
    {
        var bing = Enum.GetValues(enumType).Cast<T>(); ;
        foreach (var item in bing)
        {
            var option = new TMPro.TMP_Dropdown.OptionData();
            option.text = item.ToString();
            options3.Add(option);
        }
        dropdown.AddOptions(options3);
    }

    public void ParamChange(int index)
    {
        Param = dropdown.options[index].text;
    }
    public void OnValueChange()
    {
        //GameHandlerBase.Instance.UpdateCheat(CheatCodes, Toggle.isOn, Param);
    }
}
