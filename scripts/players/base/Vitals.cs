namespace Bonebreaker.Player
{
    public class PlayerVitals
    {
        public readonly int MAX_HEALTH;
        
        public int CurrentHealth;
        
        public PlayerVitals (int maxHealth)
        {
            MAX_HEALTH = maxHealth;
        }

        public void Damage (int amount) => CurrentHealth -= amount;

        public void Heal (int amount) => CurrentHealth += amount;
    }
}