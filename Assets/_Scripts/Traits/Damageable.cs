using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Damageable : MonoBehaviour
{
    public int scoreIncrement = 100;
    public byte health = 4;
    public GameObject explosion;
    public OnDeath OnDeathHandler;
    public Material flashMaterial;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public GameObject impactExplosion;
    public bool isDead = false;
    public bool ignoreParticalCollisions = false;
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    protected virtual void Update()
    {
    }

    //private void OnTriggerEnter2D(Collider2D otherCollider)
    //{
    //    if (otherCollider.CompareTag("Wall"))
    //    {
    //        ApplyDamage(health, true);
    //    }
    //}

    void OnCollisionEnter2D(Collision2D collision)
    {
        bool isThisObjEnemy = TryGetComponent(out Enemy _);
        bool isOtherObjEnemy = collision.gameObject.TryGetComponent(out Enemy e);
        if (isThisObjEnemy && isThisObjEnemy)
        {
            return;
        }
        else if (isOtherObjEnemy)
        {
            ApplyDamage();
            Damageable eDamageable = e.GetComponent<Damageable>();
            eDamageable.ApplyDamage(eDamageable.health);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (isDead)
        {
            return;
        }

        Instantiate(impactExplosion, gameObject.transform.position + Vector3.left * gameObject.GetComponent<Collider2D>().bounds.extents.x, Quaternion.identity);

        //if(other.GetComponentInParent<PlayerController>() != null && TryGetComponent(out PlayerController _))
        //{
        //    return;
        //}

        ApplyDamage(other.GetComponentInParent<Enemy>()?.damage ?? 1);
    }

    public void ApplyDamage(byte value = 1, bool kia = true)
    {
        if (isDead)
        {
            return;
        }

        health -= value;

        if (kia)
        {
            StartCoroutine(DoHitBlink());
        }

        spriteRenderer.material = flashMaterial;

        if (health <= 0)
        {
            isDead = true;
            if (kia)
            {
                GameManager.Instance?.OnKill(scoreIncrement, transform.position);
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            if (OnDeathHandler != null)
            {
                OnDeathHandler(this);
            };
            spriteRenderer.material = flashMaterial;
            Destroy(gameObject, kia ? 0.1f : 5f);
            GameManager.Instance?.audioSource?.PlayOneShot(GameManager.Instance?.explosionAudioClip);
        }
    }

    private IEnumerator DoHitBlink()
    {
        spriteRenderer.material = flashMaterial;
        yield return new WaitForSeconds(0.01f);
        spriteRenderer.material = originalMaterial;
    }
}

public delegate void OnDeath(Damageable thisEnemy);
