using UnityEngine;

public class AstroidController : Enemy
{
    public float force = 4;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        Vector2 targetPosition = new Vector2(-10, Random.Range(12, -12));

        rb.AddForce((targetPosition - AsVector2(rb.transform.position)) * force);
    }

    public Vector2 AsVector2(Vector3 _v) => new Vector2(_v.x, _v.y);
}