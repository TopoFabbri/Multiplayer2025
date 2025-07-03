namespace Objects
{
    public interface IDamageable
    {
        int Life { get; set; }

        void ReceiveDamage(int damage)
        {
            Life -= damage;
        }
    }
}