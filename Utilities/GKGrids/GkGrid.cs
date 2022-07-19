using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GkGrid : MonoBehaviour
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
    public GKGridObject<int> Grid;

    public bool UpdateGrid;


    public int startX, endX, startY, endY;
    private void Start()
    {
        //CreateGrid();
    }
    public void CreateGrid()
    {
        Grid = new GKGridObject<int>(GridDimentions.width, GridDimentions.height, cellSize, Origin, InitGrid);
    }
    private int InitGrid()
    {
        return 0;
    }

    public Vector3 GetWorldPos(int x, int y) => Grid.GetWorldPosOffset(x, y);

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Grid.SetValue(GKUtils.GetMouseWorldPosition(), 0);
        }
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
    private void OnDrawGizmos()
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
                Grid.DrawEditorGizmos();
            }
            
        }
        else
        {
            if (Grid != null)
            {
                Grid.DrawEditorGizmos();
            }
           
        }
       
    }
}