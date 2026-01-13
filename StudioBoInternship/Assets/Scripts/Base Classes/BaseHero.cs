using System.Collections.Generic;

namespace Base_Classes
{
    [System.Serializable]
    public class BaseHero : BaseActor
    {
        public int stamina;
        public int agility;
        public int intellect;
        
        public List<BaseAttack> magicAttacks = new List<BaseAttack>();
    }
}
