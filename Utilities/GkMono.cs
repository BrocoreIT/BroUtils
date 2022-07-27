using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GkMono : MonoBehaviour
{
    public void Log(string message)
    {
        GKUtils.Log(gameObject.name, message);
    }
    public void SetList(List<GameObject> gameObjects, bool value)
    {
        foreach (var item in gameObjects)
        {
            item.SetActive(value);
        }
    }

#if UNITY_EDITOR
    protected void DrawGizmoLables(string message)
    {
        DrawGizmoLables(message, transform.position);
    }
    protected void DrawGizmoLables( string message, Vector3 postion)
    {
        var color = Colors.DarkRed;
        DrawGizmoLables( message, postion, color);
    }
    protected void DrawGizmoLables(string message, Vector3 position, Color color)
    {
        Handles.color = color;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(position,message,style);
    }
    protected void DrawLine(Vector3 position, int thickness = 3)
    {
        DrawLine(transform.position, position, thickness);
    }  
    protected void DrawLine(Vector3 from, Vector3 position, int thickness = 3)
    {
        var color = Colors.DarkRed;
        DrawLine(from, position, color, thickness);
    }
    protected void DrawLine(Vector3 position, Color color, int thickness = 3)
    {
        DrawLine(transform.position, position,color,thickness);
    }
    protected void DrawLine(Vector3 startPosition, Vector3 endPosition, Color color, int thickness = 3)
    {
        Handles.color = color;
        var p1 = startPosition;
        var p2 = endPosition;
        Handles.DrawBezier(p1, p2, p1, p2, color, null, thickness);
    }

#endif
}
