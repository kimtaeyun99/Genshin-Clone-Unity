public struct AttackData
{
    private float damage;
    private ElementType elementType;

    public readonly float Damage => damage;
    public readonly ElementType ElementType => elementType;

    public AttackData(float damage, ElementType elementType)
    {
        this.damage = damage;
        this.elementType = elementType;
    }
}