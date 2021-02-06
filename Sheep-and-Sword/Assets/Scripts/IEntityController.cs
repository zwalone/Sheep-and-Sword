public interface IEntityController
{
    void TakeDamage(int points);
    bool IsHurting { get; }
    bool IsDead { get; }
    int ReturnCurrentHP();
    int ReturnMaxHP();
}
