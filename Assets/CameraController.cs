using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        // float moveX = Input.GetAxis("Horizontal");
        // float moveY = Input.GetAxis("Vertical");
        //
        // Vector3 movement = new Vector3(moveX, moveY, 0f) * (moveSpeed * Time.deltaTime);
        //
        // transform.Translate(movement);
    }
}
