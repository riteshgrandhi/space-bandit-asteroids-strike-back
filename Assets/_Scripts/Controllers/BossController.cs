using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer), typeof(Damageable))]
public class BossController : MonoBehaviour
{
    public AnimationCurve pathCurve;
    public Slider slider;
    public int range = 9;
    public float timeFactor = 0.5f;

    void Update()
    {
        slider.value = GetComponent<Damageable>().health;
        transform.position = new Vector2((pathCurve.Evaluate(Time.time * timeFactor) * range) - (range / 2), transform.position.y);
    }
}
