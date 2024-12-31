using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentData : MonoBehaviour
{
    public int totalScore;
    public List<int> unlockedCats;
    public int currentCat;

    public void SaveData()
    {
        SaveSystem.SaveData(this);
    }

    public void LoadData()
    {
        GameData data = SaveSystem.LoadData();

        totalScore = data.totalScore;
        unlockedCats = data.unlockedCats;
        currentCat = data.currentCat;
    }
}
