using UnityEngine;

[System.Serializable]
public class PlayerData
{

    public float recordDistance;
    public int currency;

    public PlayerData (PlayerController player)
    {
        recordDistance = player.recordDistance;
        currency = player.currencySouls;

    }

}
