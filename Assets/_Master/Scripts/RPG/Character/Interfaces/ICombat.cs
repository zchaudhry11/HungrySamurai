using UnityEngine;

public interface ICombat
{
    /// <summary>
    /// Inflicts damage to the character.
    /// </summary>
    /// <param name="dmg">Points of damage to inflict.</param>
    void Damage(int dmg);

    /// <summary>
    /// Restores HP to the character.
    /// </summary>
    /// <param name="hp">Points of HP to restore.</param>
    void Heal(int hp);
}