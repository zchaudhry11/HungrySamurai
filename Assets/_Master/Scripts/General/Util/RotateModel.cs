using UnityEngine;
using System.Collections;

public class RotateModel : MonoBehaviour 
{
	public Vector3 rotation;

    void Update()
    {
        this.transform.Rotate(rotation);
    }
}
