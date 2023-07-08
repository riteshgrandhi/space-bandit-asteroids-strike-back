using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public abstract class Enemy : Damageable
{
    protected Rigidbody2D rb;
    protected float force = 4;
    protected Vector2 direction = Vector2.down;

    public float minForce = 4;
    public float maxForce = 10;
    public byte damage = 1;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        rb.velocity = direction * force;
    }

    public void SetForceAndDirection(float force, Vector2 direction)
    {
        if (force > maxForce)
        {
            this.force = maxForce;
        }
        else if (force < minForce)
        {
            this.force = minForce;
        }
        this.direction = direction;
    }
}
