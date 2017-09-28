using UnityEngine;

public interface IObstacle
{
    /// <summary>
    /// Knocks a character back upon contact.
    /// </summary>
    /// <param name="dir">Direction of the knockback.</param>
    void Knockback(Vector3 dir);
}