[System.Serializable]
public class BaseEnemy
{
    public string name;

    public enum Type
    {
        Water,
        Fire,
        Earth,
        Air // Just examples of types if weaknesses and resistances are implemented.
    }

    public Type enemyType;
    
    public float baseHP;
    public float currentHP;
    
    public float baseMP;
    public float currentMP;
    
    public float baseATK;
    public float currentATK;
    
    public float baseDEF;
    public float currentDEF;
}
