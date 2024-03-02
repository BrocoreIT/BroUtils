using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BroGrid<T> : MonoBehaviour
{
    public RectInt GridDimentions;
    public float cellSize;

    private Vector2Int origin;
    public Vector2Int Origin
    {
        get
        {
            if (origin == null)
            {
                origin = new Vector2Int(GridDimentions.x, GridDimentions.y);
            }
            else
            {
                if (origin.x != GridDimentions.x || origin.y != GridDimentions.y)
                {
                    origin.x = GridDimentions.x;
                    origin.y = GridDimentions.y;             
                }                
            }
            return origin;
        }
    }
    public BroGridObject<T> Grid;

    public bool UpdateGrid;


    public int Size => GridDimentions.height * GridDimentions.width;
    public int startX, endX, startY, endY;
    private void Start()
    {
        //CreateGrid();
    }
    public void CreateGrid()
    {
        Grid = new BroGridObject<T>(GridDimentions.width, GridDimentions.height, cellSize, Origin, InitGrid);
    }
    private T InitGrid()
    {
        return default(T);
    }

    public Vector3 GetWorldPos(int x, int y) => Grid.GetWorldPosOffset(x, y);

    private void Update()
    {
       
    }
    [ContextMenu("PrintLineX")]
    public void GetRangeX()
    {
        string points = "|90|";
        for (int x = startX; x<=endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                string point = $"{x},{y}-";
                points += point;
            }
        }
        Debug.Log(points);
    }
    private void OnDrawGizmosSelected()
    {
        if(UpdateGrid)
        {
            if(Grid == null)
            {
                CreateGrid();
            }
            else
            {
                if(Grid.Width != GridDimentions.width || Grid.Height != GridDimentions.height || cellSize != Grid.CellSize || Grid.GridDimentions.x != GridDimentions.x || Grid.GridDimentions.y != GridDimentions.y || origin.x != GridDimentions.x || origin.y != GridDimentions.y)
                    Grid.SetGridDimentions(GridDimentions.width, GridDimentions.height, cellSize, Origin);
#if UNITY_EDITOR
                Grid.DrawEditorGizmos();
#endif
            }
            
        }
        else
        {
            if (Grid != null)
            {
#if UNITY_EDITOR
                Grid.DrawEditorGizmos();
#endif
            }

        }
       
    }
}
