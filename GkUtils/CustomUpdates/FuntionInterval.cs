using System;
using System.Collections.Generic;
using UnityEngine;


public class FuntionInterval
{
    public class MonoHelper : MonoBehaviour
    {
        public Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }


    private static List<FuntionInterval> funcList; // Holds a reference to all active timers
    private static GameObject initGameObject; // Global game object used for initializing class, is destroyed on scene change

    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FuntionInterval_Global");
            funcList = new List<FuntionInterval>();
        }
    }



    // Persist through scene loads
    public static FuntionInterval Create_Global(Action action, Func<bool> testDestroy, float timer)
    {
        FuntionInterval funtionInterval = Create(action, testDestroy, timer, "", false, false, false);
        UnityEngine.Object.DontDestroyOnLoad(funtionInterval.gameObject);
        return funtionInterval;
    }


    // Trigger [action] every [timer], execute [testDestroy] after triggering action, destroy if returns true
    public static FuntionInterval Create(Action action, Func<bool> testDestroy, float timer)
    {
        return Create(action, testDestroy, timer, "", false);
    }

    public static FuntionInterval Create(Action action, float timer)
    {
        return Create(action, null, timer, "", false, false, false);
    }

    public static FuntionInterval Create(Action action, float timer, string functionName)
    {
        return Create(action, null, timer, functionName, false, false, false);
    }

    public static FuntionInterval Create(Action callback, Func<bool> testDestroy, float timer, string functionName, bool stopAllWithSameName)
    {
        return Create(callback, testDestroy, timer, functionName, false, false, stopAllWithSameName);
    }

    public static FuntionInterval Create(Action action, Func<bool> testDestroy, float timer, string functionName, bool useUnscaledDeltaTime, bool triggerImmediately, bool stopAllWithSameName)
    {
        InitIfNeeded();

        if (stopAllWithSameName)
        {
            StopAllFunc(functionName);
        }

        GameObject gameObject = new GameObject("FuntionInterval Object " + functionName, typeof(MonoHelper));
        FuntionInterval FuntionInterval = new FuntionInterval(gameObject, action, timer, testDestroy, functionName, useUnscaledDeltaTime);
        gameObject.GetComponent<MonoHelper>().OnUpdate = FuntionInterval.Update;

        funcList.Add(FuntionInterval);

        if (triggerImmediately) action();

        return FuntionInterval;
    }




    public static void RemoveTimer(FuntionInterval funcTimer)
    {
        InitIfNeeded();
        funcList.Remove(funcTimer);
    }
    public static void StopTimer(string _name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == _name)
            {
                funcList[i].DestroySelf();
                return;
            }
        }
    }
    public static void StopAllFunc(string _name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == _name)
            {
                funcList[i].DestroySelf();
                i--;
            }
        }
    }
    public static bool IsFuncActive(string name)
    {
        InitIfNeeded();
        for (int i = 0; i < funcList.Count; i++)
        {
            if (funcList[i].functionName == name)
            {
                return true;
            }
        }
        return false;
    }




    private GameObject gameObject;
    private float timer;
    private float baseTimer;
    private bool useUnscaledDeltaTime;
    private string functionName;
    public Action action;
    public Func<bool> testDestroy;


    private FuntionInterval(GameObject gameObject, Action action, float timer, Func<bool> testDestroy, string functionName, bool useUnscaledDeltaTime)
    {
        this.gameObject = gameObject;
        this.action = action;
        this.timer = timer;
        this.testDestroy = testDestroy;
        this.functionName = functionName;
        this.useUnscaledDeltaTime = useUnscaledDeltaTime;
        baseTimer = timer;
    }

    public void SkipTimerTo(float timer)
    {
        this.timer = timer;
    }

    public void SetBaseTimer(float baseTimer)
    {
        this.baseTimer = baseTimer;
    }

    public float GetBaseTimer()
    {
        return baseTimer;
    }

    private void Update()
    {
        if (useUnscaledDeltaTime)
        {
            timer -= Time.unscaledDeltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            action();
            if (testDestroy != null && testDestroy())
            {
                //Destroy
                DestroySelf();
            }
            else
            {
                //Repeat
                timer += baseTimer;
            }
        }
    }

    public void DestroySelf()
    {
        RemoveTimer(this);
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}
