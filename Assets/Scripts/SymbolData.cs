using UnityEngine;

/// <summary>
/// Describes one slot symbol. Create one asset per symbol
/// (right-click in Project > Create > Slot > Symbol).
/// </summary>
[CreateAssetMenu(menuName = "Slot/Symbol")]
public class SymbolData : ScriptableObject
{
    public Sprite sprite;               // the picture shown on the reel
    public int weight = 10;             // bigger number = appears more often
    public int payoutMultiplier = 5;    // win amount = bet x this number
    public bool isWild;  
    
}