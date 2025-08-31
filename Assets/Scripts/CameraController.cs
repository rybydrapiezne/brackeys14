using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector3 oldPos;

    void Start()
    {
        oldPos = transform.position;
    }

    void Update()
    {
        if (TurnController.Instance.isMoving)
        {
            oldPos = transform.position;
            return;    
        }

        float move = Input.GetAxis("Horizontal");

        if (move == 0.0f)
        {
            transform.position = oldPos;
        }
        
        Vector3 movement = new Vector3(move, 0f, 0f) * (moveSpeed * Time.deltaTime);
        
        transform.Translate(movement);
    }
}
