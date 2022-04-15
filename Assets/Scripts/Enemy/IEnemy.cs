using UnityEngine;

namespace ViralVial.Enemy
{
    public interface IEnemy
    {
        float Health { get; } //enemy health
        float Speed { get; } //enemy speed
        float Damage { get; } //enemy damage
        float Experience { get; } //enemy experience given on death 
        GameObject Target { get; set; }
        bool IsDead { get; }

        //function for enemy to inflict damage (they are an enemy)
        void InflictDamage();
        //function to make enemy take damage
        void TakeDamage(float damage); //DamageEnemy may be better?
    }
}
