using UnityEngine;

public class StatManager : MonoBehaviour, IStats
{
    [Header("Life Stats")]
    public int HP = 100;
    public int maxHP = 100;

    public int Stamina = 100;
    public int maxStamina = 100;

    public int MP = 100;
    public int maxMP = 100;

    [Header("Base Character Stats")]
    public int Strength = 1;
    public int Defense = 1;
    public int Magic = 1;
    public int Luck = 1;
    public float critDamageBoost = 1.1f; // Damage bonus that is multiplied with output damage
    public int damagePower = 1;

    [Header("Secondary Stats")]
    public int level = 1;
    private int xp = 0;
    private int nextLvlXP = 1;
    public int money = 0;

    public int GetDamagePower()
    {
        // Calculate damage
        int bound1 = Random.Range(Strength, Strength * 5);
        int bound2 = Random.Range(Strength, Strength * 5);

        // If bound1 is greater than bound2 then swap
        if (bound1 > bound2)
        {
            int temp = bound1;

            bound1 = bound2;
            bound2 = temp;
        }

        float damage = Random.Range(bound1, bound2);

        int crit = Random.Range(0, 100);

        if (crit < Luck)
        {
            damage *= critDamageBoost;
        }

        // Update damage output
        damagePower = (int)damage;

        return damagePower;
    }

    /// <summary>
    /// Adds number of levels to a character's current level.
    /// </summary>
    /// <param name="numLevels">Number of level ups to give.</param>
    public void LevelUp(int numLevels)
    {
        level++;
    }

    /// <summary>
    /// Adds experience points to a character.
    /// </summary>
    /// <param name="points">Number of XP points to give.</param>
    public void GainXP(int points)
    {
        xp += points;
    }

    /// <summary>
    /// Returns the current level of a character.
    /// </summary>
    public int GetLevel()
    {
        return level;
    }

    /// <summary>
    /// Returns the current amount of XP of a character.
    /// </summary>
    public int GetCurrXP()
    {
        return xp;
    }

    /// <summary>
    /// Returns the XP needed to level up.
    /// </summary>
    public int GetXPToLevelUp()
    {
        return nextLvlXP;
    }
}