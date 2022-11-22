using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;
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


public static class GizmoExtentions
{

#if UNITY_EDITOR
    public static void ShowName(this Transform theObject)
    {
        theObject.ShowName(Colors.Red);
    }

    public static void ShowName(this Transform theObject, Color color)
    {
        Handles.color = color;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(theObject.transform.position, theObject.name, style);
    }
#endif
    public static void DrawWireCube(this Transform transform)
    {
        DrawWireCube(transform, Colors.Red) ;
    }
    public static void DrawWireCube(this Transform transform, Color color)
    {
        DrawWireCube(transform, Vector3.zero,color);
    }
    public static void DrawWireCube(this Transform transform, Vector3 offset, Color color, float sizeMod = 1)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.position + offset, transform.localScale * sizeMod);
    }


    public static void DrawLine(this Transform transform, Vector3 position, int thickness = 3)
    {
        DrawLine(transform.position, position, thickness);
    }
    public static void DrawLine(this Vector3 from, Vector3 position, int thickness = 3)
    {
        var color = Colors.DarkRed;
        DrawLine(from, position, color, thickness);
    }
    public static void DrawLine(this Transform transform, Vector3 position, Color color, int thickness = 3)
    {
        DrawLine(transform.position, position, color, thickness);
    }
    public static void DrawLine(this Vector3 startPosition, Vector3 endPosition, Color color, int thickness = 3)
    {
        Handles.color = color;
        var p1 = startPosition;
        var p2 = endPosition;
        Handles.DrawBezier(p1, p2, p1, p2, color, null, thickness);
    }


}