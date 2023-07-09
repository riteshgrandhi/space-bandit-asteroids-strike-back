using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Damageable))]
public class BossController : MonoBehaviour
{
    public AnimationCurve pathCurve;
    public int range = 9;
    public float timeFactor = 0.5f;

    void Update()
    {
        transform.position = new Vector2((pathCurve.Evaluate(Time.time * timeFactor) * range) - (range / 2), transform.position.y);
    }
}
