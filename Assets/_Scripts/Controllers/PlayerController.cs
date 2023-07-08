using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Damageable
{
    public PlayerConfig playerConfig;
    public SpriteRenderer shieldSprite;
    private static Vector2 BOUNDS = new Vector2(7.5f, 7);
    private Rigidbody2D rb;
    private ParticleSystem bulletParticleSystem;
    private PickableType? activePickable;


    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        bulletParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    protected override void Update()
    {
        base.Update();
        //rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * playerConfig.speed;


        var xValidPosition = Mathf.Clamp(rb.position.x, -BOUNDS.x, BOUNDS.x);
        var yValidPosition = Mathf.Clamp(rb.position.y, -BOUNDS.y, BOUNDS.y);
        rb.position = new Vector2(xValidPosition, yValidPosition);

        //if (Input.GetButtonDown("Fire1"))
        //{
        //    bulletParticleSystem.Play();
        //}
        //else if(Input.GetButtonUp("Fire1"))
        //{ 
        //    bulletParticleSystem.Stop();
        //}
        bulletParticleSystem.Play();
    }

    public override void ApplyDamage(byte value = 1, bool kia = true)
    {
        if (activePickable != PickableType.SHIELD)
        {
            base.ApplyDamage(value, kia);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy e))
        {
            ApplyDamage();
            e.ApplyDamage(e.health);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool isSuccess = other.gameObject.TryGetComponent(out Pickable p);
        if (isSuccess)
        {
            GameManager.Instance?.audioSource?.PlayOneShot(GameManager.Instance?.pickUpAudioClip);
            Destroy(other.gameObject);
            activePickable = p.pickableType;
            switch (activePickable)
            {
                case PickableType.SHIELD:
                    shieldSprite.enabled = true;
                    StartCoroutine(DeactivateShield());
                    break;
                case PickableType.HEALTH:
                    health++;
                    break;
            }
        }
    }

    private IEnumerator DeactivateShield()
    {
        yield return new WaitForSeconds(5f);
        shieldSprite.enabled = false;
        activePickable = null;
    }
}