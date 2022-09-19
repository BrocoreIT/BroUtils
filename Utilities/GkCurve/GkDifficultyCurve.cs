using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DifficultyCurve",menuName ="Data/DiffcultyuCurve")]
public class GkDifficultyCurve : ScriptableObject
{
    public GkCurve difficultyCurve;
    public GkCurve playerProgression;

    public CustomList<string, LevelCurves> LevelCurves;


    public CustomList<string, GkCurve> BattlersCurve;

    public LevelCurves GetLevelCurves(float val)
    {
       
        foreach(var item in LevelCurves.List)
        {
            if (item.value.CheckInRange(val))
            {
                return item.value;
            } 
                
        }
        return null;
    }

    public GkCurve GetLevelCurve(float val)
    {
        var lCurves = GetLevelCurves(val);


        var shuffled = lCurves.gkCurves.GetRandomShuffled();

        return shuffled;
    }

    public float value;
    public float ToMin, ToMax, FromMin, FromMax;
    [ContextMenu("MapTest")]
    public void MapTest()
    {
        var clamped = Mathf.Clamp(value, ToMin, ToMax);
        var maped = value.Map(FromMin, FromMax, ToMin, ToMax);
        GKUtils.Log("Map Test", $"{value} is now:\nClamped : {clamped} \nMaped : {maped} \n from :: {FromMin} : {FromMax} \n-> to :: {ToMin} : {ToMax}");
    }


}




[System.Serializable]
public class LevelCurves
{
    public Vector2 range;
    public List<GkCurve> gkCurves;

    
    public bool CheckInRange(float val)
    {
        return range.CheckInRange(val);
    }
}
