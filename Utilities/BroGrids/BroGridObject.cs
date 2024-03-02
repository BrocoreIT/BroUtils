using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class BroGridObject<T>
{
    public RectInt GridDimentions;
    public float CellSize;
    [SerializeField]
    private T[,] gridArray;
    private TextMesh[,] textMeshes;
    private Vector3 origin;

    public bool inGameDebug;
    public int Width { get { return GridDimentions.width; } set { GridDimentions.width = value; } }

    public int Height { get { return GridDimentions.height; } set { GridDimentions.height = value; } }

    public T[,] GridArray { get => gridArray; set => gridArray = value; }
    public Vector3 Origin { get => origin; set => origin = value; }

    public BroGridObject (int width,int height, float cellSize, Vector2Int originPos, Func<T> createGridObj)
    {
        SetGridDimentions(width, height, cellSize, originPos);
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                GridArray[x, y] = createGridObj();
            }
        }

        DrawGizmo();
    }

    public void SetGridDimentions(int width, int height, float cellSize, Vector2Int originPos) 
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        GridArray = new T[width, height];
        textMeshes = new TextMesh[width, height];
        GridDimentions.x = originPos.x;
        GridDimentions.y = originPos.y;

        Origin = new Vector3(originPos.x,0, originPos.y);
    }

    public bool SetValue(Vector3 worldPos, T value)
    {

        var pos = GetGridPos(worldPos);
        return SetValue(pos.x, pos.y, value);
    }

    public bool SetValue(int x, int y, T value)
    {
        //Log($"set value at {x},{y} : {value}");
        if (x >= 0 && y >= 0 && x < Width && y<Height)
        {
            //Log($"set value at {x},{y} : {value} :: x: { GetWorldPosOffset(x, y).x} y: { GetWorldPosOffset(x, y).y} ");
           
            GridArray[x, y] = value;
            if(inGameDebug)
                textMeshes[x, y].text = value.ToString();

            return true;
        }
        return false;
    }
    public T GetValue(Vector3 worldPos)
    {
        var pos = GetGridPos(worldPos);
        return GetValue(pos.x, pos.y);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            var value = GridArray[x, y];
            Log($"value at {x},{y} : {value}");
            return value;
        }
        return default;
    }

    public void SetValues(Func<T> value)
    {
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                SetValue(x, y, value());
            }
        }
    }
    public void SetValues(T value)
    {
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                SetValue(x, y, value);
            }
        }
    }
    public void GetValues(System.Action<T> value)
    {
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                var item = GridArray[x,y];
                Log($"value at {x},{y} : {item}");
                value?.Invoke(item);
            }
        }
    }

    public void GetValues(System.Action<T,int,int> value)
    {
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                var item = GridArray[x, y];
                //Log($"value at {x},{y} : {item}");
                value?.Invoke(item,x,y);
            }
        }
    }

    public void GetIndexes(System.Action<int, int> value)
    {
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                value?.Invoke(x, y);
            }
        }
    }
    public Vector3 GetWorldPos(Vector2Int vector2Int)
    {
        return new Vector3(vector2Int.x,0, vector2Int.y) * CellSize + Origin;
    }
    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x,0, y) * CellSize  + Origin;
    }
    public Vector3 GetWorldPosOffset(Vector2Int vector2Int)
    {
        return (new Vector3(vector2Int.x,0, vector2Int.y) * CellSize) + (new Vector3(CellSize, 0,CellSize) * 0.5f) + Origin;
    }
    public Vector3 GetWorldPosOffset(int x, int y)
    {
        return (new Vector3(x,0, y) * CellSize) + (new Vector3(CellSize,0,CellSize) * 0.5f) + Origin;
    }

    public Vector2Int GetGridPos(Vector3 worldPos)
    {
        Vector2Int v2 = new Vector2Int(0, 0);

        v2.x = Mathf.FloorToInt((worldPos - Origin).x / CellSize);
        v2.y = Mathf.FloorToInt((worldPos - Origin).y / CellSize);

        return v2;

    }
    public Vector2Int GetRandGridPosFixedY(int y)
    {
        var randX = UnityEngine.Random.Range(0, Width);

        return new Vector2Int(randX, y);
    }

    public Vector2Int GetRandGridPosFixedX(int x)
    {
        var randY = UnityEngine.Random.Range(0, Height);

        return new Vector2Int(x, randY);
    }

    public Vector2Int GetRandGridPos()
    {
        var randY = UnityEngine.Random.Range(0, Height);
        var randX = UnityEngine.Random.Range(0, Width);

        return new Vector2Int(randX, randY);
    }

    public Vector3 GetRandomWorldPosFixedX(int x)
    {
        var randY = UnityEngine.Random.Range(0, Height);

        return GetWorldPosOffset(x, randY);
    }
    public Vector3 GetRandomWorldPosFixedY(int y)
    {
        var randX = UnityEngine.Random.Range(0, Width);

        return GetWorldPosOffset(randX, y);
    }

    public Vector3 GetRandomWorldPos()
    {
        var randX = UnityEngine.Random.Range(0, Width);
        var randY = UnityEngine.Random.Range(0, Height);

        return GetWorldPosOffset(randX, randY);
    }
    private void DrawGizmo()
    {
        if (!inGameDebug)
            return;
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {

            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                textMeshes[x,y] = BroUtils.CreateWorldText(GridArray[x, y].ToString(), null, GetWorldPosOffset(x, y), 20, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.black, 100);
                Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.black, 100);
            }
        }
        Debug.DrawLine(GetWorldPos(0, Height), GetWorldPos(Width, Height), Color.black, 100);
        Debug.DrawLine(GetWorldPos(Width, 0), GetWorldPos(Width, Height), Color.black, 100);
    }


    public void Log(string message, bool isError = false)
    {
        if(isError)
        {
            Debug.LogError($"<<GK Grid Obj>> {message}");
            return;
        }

        Debug.Log($"<<GK Grid Obj>> {message}");
    }

#if UNITY_EDITOR
    public void DrawEditorGizmos()
    {
        if (gridArray == null)
            GridArray = new T[Width, Height];

        Gizmos.color = Colors.DarkRed;
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {

            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                Vector3 position = GetWorldPosOffset(x, y);
                Vector3 pos = position - new Vector3(0.2f, 0.4f, 0);
             
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Colors.Yellow;
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
                Handles.Label(pos, $"({x},{y})", style);
                //GKUtils.CreateWorldText(Grid.GridArray[x, y].ToString(), null, Grid.GetWorldPosOffset(x, y), 20, Color.white, TextAnchor.MiddleCenter);
                GetWorldPos(x, y).DrawLine(GetWorldPos(x, y + 1));
                GetWorldPos(x, y).DrawLine(GetWorldPos(x + 1, y));
                //Gizmos.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1));
                //Gizmos.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y));
            }
        }
        GetWorldPos(0, Height).DrawLine(GetWorldPos(Width, Height));
        GetWorldPos(Width, 0).DrawLine(GetWorldPos(Width, Height));
        //Gizmos.DrawLine(GetWorldPos(0, Height), GetWorldPos(Width, Height));
        //Gizmos.DrawLine(GetWorldPos(Width, 0), GetWorldPos(Width, Height));
    }
#endif

}
