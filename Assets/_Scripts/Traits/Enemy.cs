using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : Damageable
{
    private float speed = 0;
    protected Rigidbody2D rb;
    public byte damage = 1;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        rb.velocity = Vector2.left * speed;
    }
    
    public void SetSpeed(float currentSpeed) => this.speed = currentSpeed;
}
