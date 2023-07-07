using UnityEngine;

public class Pickable : MonoBehaviour
{
    public PickableType pickableType;
    private float speed;
    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}

public enum PickableType
{
    SHIELD,
    HEALTH,
    LASER,
    DOUBLE_BULLET,
    TRIPLE_BULLET,
}
