namespace Base_Classes
{
    [System.Serializable]
    public class BaseEnemy : BaseActor
    {
        public enum Type
        {
            Water,
            Fire,
            Earth,
            Air // Just examples of types if weaknesses and resistances are implemented.
        }

        public Type enemyType;
    }
}
