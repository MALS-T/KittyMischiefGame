using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class GameData
{
    public int totalScore;
    public List<int> unlockedCats;
    public int currentCat;

    public GameData(CurrentData currentData)
    {
        totalScore = currentData.totalScore;
        unlockedCats = currentData.unlockedCats;
        currentCat = currentData.currentCat;
    }
}
