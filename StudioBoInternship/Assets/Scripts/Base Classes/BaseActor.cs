using System.Collections.Generic;

namespace Base_Classes
{
    public class BaseActor
    {
        public string ActorName;
    
        public float BaseHp;
        public float CurrentHp;
    
        public float BaseMp;
        public float CurrentMp;
    
        public float Attack;
    
        public float Defence;
        
        public List<BaseAttack> ActorAttacks = new List<BaseAttack>();
    }
}
