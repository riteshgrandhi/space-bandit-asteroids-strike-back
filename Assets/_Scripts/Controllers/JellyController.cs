using UnityEngine;

public class JellyController : Enemy
{
    public float speed = 10;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        SetSpeed(speed);
    }

    public Vector2 AsVector2(Vector3 _v) => new Vector2(_v.x, _v.y);
}