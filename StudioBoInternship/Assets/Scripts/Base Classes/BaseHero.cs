using System.Collections.Generic;

namespace Base_Classes
{
    [System.Serializable]
    public class BaseHero : BaseActor
    {
        public float strength;
        public float agility;
        public float wisdom;
        
        public List<BaseAttack> magicAttacks = new List<BaseAttack>();
    }
}
