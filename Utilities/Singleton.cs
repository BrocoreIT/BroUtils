using UnityEngine;

/// <summary>
/// Written by GK. 
/// Base singelton class.
/// with option (DoNotDestroy, can be set in inspector) to make it persistant using DontDestroyOnLoad
/// </summary>
public class Singleton<T> : BroMono where T : Component
{
    public bool DoNotDestroy;
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (DoNotDestroy)
                DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
