using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="GkCurve",menuName="Data/GkCurves")]
public class GkCurve : ScriptableObject
{
  
    [Header("Description")]
    public string valueIs;
    public string timeIs;
    [Space]
    public AnimationCurve gkCurve;
    [Space]

    [Header("AutoSetup")]
    public bool AutoMinMax;
    [SerializeField]
    private float minValue;
    [SerializeField]
    private float maxValue;  
    
    [SerializeField]
    private float minTime;
    [SerializeField]
    private float maxTime;

    public float MinValue
    {
        get => minValue; 
        set => minValue = value; 
    }
    public float MaxValue 
    { 
        get => maxValue; 
        set => maxValue = value; 
    }
    public float MinTime { get => minTime; set => minTime = value; }
    public float MaxTime { get => maxTime; set => maxTime = value; }


    public int TotalValues => gkCurve.length - 1;

    private void OnEnable()
    {
        if( AutoMinMax
            && gkCurve !=null 
            && gkCurve.length !=0)
        {
            SetValues();
        }
    }
    [ContextMenu("Set Values")]
    public void SetValues()
    {
        MinValue = gkCurve.keys[0].value;
        MaxValue = gkCurve.keys[TotalValues].value;

        MinTime = gkCurve.keys[0].time;
        MaxTime = gkCurve.keys[TotalValues].time;
    }
    [ContextMenu("Set Player Values")]
    public void SetPlayerValues()
    {
        int val = -1;
        gkCurve.keys = new Keyframe[(int)maxTime];
        for (int i = 0; i < maxTime; i++)
        {
           
            if(i%2==0)
                val++;

                gkCurve.AddKey(i, val);;
                

            
        }/*
        MinValue = gkCurve.keys[0].value;
        MaxValue = gkCurve.keys[TotalValues].value;

        MinTime = gkCurve.keys[0].time;
        MaxTime = gkCurve.keys[TotalValues].time;*/
    }
    public float interval;
    public float CheckOperator = 0;
    public float incementVal;
    public float incementValOther;
    public float minVal;
    [ContextMenu("Make Curve")]
    public void CurveGenrator()
    {
        for(int i =0; i< gkCurve.length;i++)
        {
            gkCurve.RemoveKey(i);

        }
        gkCurve = new AnimationCurve();

        
        float val = 0 ;
        for (int i = 0; i < maxTime; i++)
        {
            if (i % interval == CheckOperator)
                val += incementVal;
            else
                val += incementValOther;

            Debug.Log($"{i} :: {val}");
            if (true)
            {
                val = val.Map(minTime,maxTime,0,maxTime + (maxTime * 25/100));
            }
            var at = gkCurve.AddKey(i, val);

            

            Debug.Log($"at {at} key is {i} :: val is {val}");
        }
    }

    public Vector2 mutateRange;
    [ContextMenu("Mutate")]
    public void Mutate()
    {
        for (int i = 0; i < gkCurve.length; i++)
        {
            gkCurve.keys[i].time += mutateRange.GetRandom();
            gkCurve.keys[i].value += mutateRange.GetRandom();

        }


    }

    public Vector2 timeR;
    public Vector2 valueR;
    [ContextMenu("Readjust")]
    public void ReAdjust()
    {

        Keyframe[] keyframe = new Keyframe[gkCurve.length];
        for (int i = 0; i < gkCurve.length; i++)
        {
            keyframe[i].time = gkCurve.keys[i].time.Map(minTime, maxTime, timeR.x, timeR.y) ;
            float v = gkCurve.keys[i].value.Map(minVal, maxValue, valueR.x, valueR.y);
            Debug.Log(v);
            keyframe[i].value = v;

        }

        for (int i = 0; i < gkCurve.length; i++)
        {
            gkCurve.RemoveKey(i);

        }
        gkCurve = new AnimationCurve();
        for (int i = 0; i < keyframe.Length; i++)
        {
            gkCurve.AddKey(keyframe[i]);

        }


    }
    public float GetMinTime()
    {
        return gkCurve.keys[0].time;
    }
    public float Remaped01(float time, float timeFromMin, float timeFromMax)
    {
        return Remap(time, 0, 1f, timeFromMin,  timeFromMax);
    }

    public float Remap(float time, float minTo, float maxTo, float timeFromMin, float timeFromMax)
    {
        var remapTime = time.Map(timeFromMin, timeFromMax, MinTime, MaxTime);
        Debug.Log($"remapTime {remapTime}");
        return gkCurve.Evaluate(remapTime).Map(minValue, maxValue, minTo, maxTo);
    } 
    
    
}
