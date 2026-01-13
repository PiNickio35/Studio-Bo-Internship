using System.Collections.Generic;

namespace Base_Classes
{
    public class BaseActor
    {
        public string actorName;
    
        public float baseHP;
        public float currentHP;
    
        public float baseMP;
        public float currentMP;
    
        public float baseATK;
        public float currentATK;
    
        public float baseDEF;
        public float currentDEF;
        
        public List<BaseAttack> actorAttacks = new List<BaseAttack>();
    }
}
