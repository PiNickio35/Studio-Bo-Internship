using UnityEngine;

namespace Base_Classes
{
    [CreateAssetMenu(fileName = "BaseAttack", menuName = "Scriptable Objects/BaseAttack")]
    [System.Serializable]
    public class BaseAttack : ScriptableObject
    {
        public string attackName;
        public float attackDamage;
        public float attackCost;
    }
}
