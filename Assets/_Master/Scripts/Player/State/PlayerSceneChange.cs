using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneChange : MonoBehaviour
{
    private void Start()
    {
        print("test " + SceneManager.GetActiveScene().name);
    }
}