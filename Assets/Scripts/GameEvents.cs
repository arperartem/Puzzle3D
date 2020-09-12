using System;
public class GameEvents 
{
    public static Action<int> OnSwap;
    public static Action<int> OnNewBestScore;

    public static Action LoadNewLevel;
    public static Action OnLoadNewLevel;

}
