using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoGKUpdate : MonoBehaviour
{
    public Action OnUpdate;

    private void Update()
    {
        OnUpdate?.Invoke();
    }
}