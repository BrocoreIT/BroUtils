using UnityEngine;

/// <summary>
///  Written by GK. For Data management.
///  How to use - attach static scriptable objects with data to access from anywhere
///  eg - 
///  [SerializeField]
///  private DataExample dataExample;
///  public static DataExample DataExample => Instance.dataExample;
/// </summary>

public class DataManager : Singleton<DataManager>
{
<<<<<<< HEAD
    [SerializeField]
    private ProcessorBank processorBank;

    public static ProcessorBank ProcessorBank => Instance.processorBank;


    public static BingoData BingoData => ProcessorBank.CurrentModule.BingoData;






=======
   
>>>>>>> c26b8242fc9839e33fd40149cee1e633e895f015
}
