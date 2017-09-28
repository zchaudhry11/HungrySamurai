using UnityEngine;

public interface IPickup
{
    /// <summary>
    /// Adds the item to a character's inventory.
    /// </summary>
    void Pickup();

    /// <summary>
    /// Destroys the item.
    /// </summary>
    void Destroy();
}