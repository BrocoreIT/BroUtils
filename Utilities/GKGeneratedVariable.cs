using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate Action<float> Authoured(int diff);

[System.Serializable]
public class GKGeneratedVariable<T> 
{

    public GenerationType generationType;

    public T initialVal;
    [SerializeField]
    private T value;

    public GenRange<T> genRange;

    public T Value { get => value; set => this.value = value; }



    public void SetValue(Func<T> random, Func<T> authored)
    {
        switch (generationType)
        {
            case GenerationType.Fixed:
                Value = initialVal;
                break;
            case GenerationType.Random:
                Value = random();
                break;
            case GenerationType.Authored:
                if (authored != null)
                { Value = authored(); }

               
                break;
        }
        
    }

}

[System.Serializable]
public class GenRange<T>
{
    public T min;
    public T max;
}