using UnityEngine;

public class BoomerangSpin : MonoBehaviour
{
    Vector3 spinAxis = Vector3.up;
    float spinSpeed = 360f; // Degrees per second

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinAxis * spinSpeed * Time.deltaTime, Space.Self);
    }
}
