using System.Collections.Generic;
using UnityEngine;

namespace Base_Classes
{
    [CreateAssetMenu(fileName = "BaseRegion", menuName = "Scriptable Objects/BaseRegion")]
    public class BaseRegion : ScriptableObject
    {
        public int maxEnemies = 4;
        public string battleScene;
        public Sprite battleBackground;
        public List<GameObject> possibleEnemies = new List<GameObject>();
    }
}
