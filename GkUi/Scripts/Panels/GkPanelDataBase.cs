using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GkPanelDataBase : ScriptableObject
{
    public GkPanel GkPanelPrefab;
    [SerializeField]
    private GkPanel runtimePanel;

    public GkPanel RuntimePanel { get => runtimePanel; set => runtimePanel = value; }

    public void UpdateDisplayContent(object content)
    {
        RuntimePanel.UpdateContent(content);
    }

    public void ResetContent()
    {
        RuntimePanel.ResetContent();
    }

    public virtual void ShowConent(bool value = false)
    {
        RuntimePanel.gameObject.SetActive(value);
    }
}
