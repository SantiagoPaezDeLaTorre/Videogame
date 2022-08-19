using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;

[CreateAssetMenu(menuName = "myGame2/TaliObjects")]
public class TaliObjects : ScriptableObject {
    
    public Dictionary<int, string> talismans = new Dictionary<int, string>();
    //[System.NonSerialized]
    //public UnityEvent<int> taliUsed;

    public int earthCount = 0;
    public int windCount = 0;
    public int fireCount = 0;
    public int waterCount = 0;

    private void OnEnable() {
        createTaliDictionary();
    }

    public void createTaliDictionary() {
        talismans.Add(1, "Aire");
        talismans.Add(2, "Agua");
        talismans.Add(3, "Tierra");
        talismans.Add(4, "Fuego");
    }

    public void addUse(int value) {
        switch (value) {
            case 1:
                earthCount += 1;
                break;
            case 2:
                windCount += 1;
                break;
            case 3:
                fireCount += 1;
                break;
            case 4:
                waterCount += 1;
                break;
        }
    }
    
}
