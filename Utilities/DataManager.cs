using UnityEngine;

/// <summary>
///  Written by GK. For Data management.
///  How to use - attach static scriptable objects with data to access from anywhere
///  eg - 
///  [SerializeField]
///  private DataExample dataExample;
///  public static DataExample DataExample => Instance.dataExample;
/// </summary>

public class DataManager : BroSingleton<DataManager>
{
    [SerializeField]
    private GameData gameData;
    public static GameData GameData => Instance.gameData;

    public static bool HasLights => GameData.HasLight;
}
