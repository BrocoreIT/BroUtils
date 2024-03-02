using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName="GkCurve",menuName="Data/GkCurves")]
public class BroCurve : ScriptableObject
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
            GetValues();
        }
    }
    [ContextMenu("Get Values")]
    public void GetValues()
    {
        MinValue = gkCurve.keys[0].value;
        MaxValue = MaxVal();

        MinTime = gkCurve.keys[0].time;
        MaxTime = gkCurve.keys[TotalValues].time;
    }
  


    public float MaxVal()
    {
        var val = 0.0f;

        foreach(var item in gkCurve.keys)
        {
            if(item.value> val) val = item.value;
        }
        return val;
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

         /*   if (true)
            {
                val = val.Map(minTime,maxTime,0,maxTime + (maxTime * 25/100));
            }*/
            var at = gkCurve.AddKey(i, val);

            

            this.Log($"at index {at} : key is {i} :: val is {val}");
        }
    }

    public Vector2 mutateRange;
    [ContextMenu("Mutate")]
    public void Mutate()
    {
        Keyframe[] keyframe = new Keyframe[gkCurve.length];
        for (int i = 0; i < gkCurve.length; i++)
        {
            this.Log("b4 " + gkCurve.keys[i].time.ToString() + " :: " + (gkCurve.keys[i].time + mutateRange.GetRandom()));
            keyframe[i].time = gkCurve.keys[i].time+ mutateRange.GetRandom();
            keyframe[i].value = gkCurve.keys[i].value+ mutateRange.GetRandom();
           
        }
        for (int i = 0; i < gkCurve.length; i++)
        {
            gkCurve.RemoveKey(i);

        }
        gkCurve = new AnimationCurve();
        for (int i = 0; i < keyframe.Length; i++)
        {
            gkCurve.AddKey(keyframe[i]);
            this.Log("now " + gkCurve.keys[i].time.ToString());
        }
        this.Log("Values are mutated");

    }

    public Vector2 timeR;
    public Vector2 valueR;
    [ContextMenu("Readjust")]
    public void ReAdjust()
    {
        GetValues();
        Keyframe[] keyframe = new Keyframe[gkCurve.length];
        for (int i = 0; i < gkCurve.length; i++)
        {
            keyframe[i].time = gkCurve.keys[i].time.Map(minTime, maxTime, timeR.x, timeR.y) ;
          
            float v = gkCurve.keys[i].value.Map(MinValue, MaxValue, valueR.x, valueR.y);
      
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

        GetValues();
        this.Log("Values are readjusted");
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

    public float Evaluate(float time)
    {
        return gkCurve.Evaluate(time);
    }

}
