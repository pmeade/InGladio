namespace lib
{
    public interface Target
    {
        void TakeDamage(int amount, PlayerController dealer);
        Place Location { get; }
        Play ActivePlay { get; }
    }
}