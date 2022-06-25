namespace lib
{
    public interface Target
    {
        void TakeDamage(int amount, PlayerController dealer);
        void Move(Place place);
    }
}