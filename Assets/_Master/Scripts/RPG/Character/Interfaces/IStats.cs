public interface IStats
{
    /// <summary>
    /// Adds number of levels to a character's current level.
    /// </summary>
    /// <param name="numLevels">Number of level ups to give.</param>
    void LevelUp(int numLevels);

    /// <summary>
    /// Adds experience points to a character.
    /// </summary>
    /// <param name="points">Number of XP points to give.</param>
    void GainXP(int points);

    /// <summary>
    /// Returns the current level of a character.
    /// </summary>
    int GetLevel();

    /// <summary>
    /// Returns the current amount of XP of a character.
    /// </summary>
    int GetCurrXP();

    /// <summary>
    /// Returns the XP needed to level up.
    /// </summary>
    int GetXPToLevelUp();
}