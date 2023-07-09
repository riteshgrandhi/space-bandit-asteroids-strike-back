using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    private static int NUM_RAYS = 12;
    public float viewConeRadius = 15;
    public Vector2 targetVector { get; private set; }

    public string[] enemyTags = new string[] { "Enemy", "Boss" };
    public string[] wallTags = new string[] { "Wall" };
    public string[] pickupTags = new string[] { "Pickup" };

    Vector2[] weightDirections = new Vector2[NUM_RAYS];
    Vector2[] decisionWeights = new Vector2[NUM_RAYS];

    public AnimationCurve pickupWeightCurve = AnimationCurve.Constant(0, 1, 5);
    public AnimationCurve enemyWeightCurve = AnimationCurve.Constant(0, 1, 5);
    public AnimationCurve wallWeightCurve = AnimationCurve.Constant(0, 1, 5);

    void Awake()
    {
        for (int i = 0; i < NUM_RAYS; i++)
        {
            weightDirections[i] = Quaternion.AngleAxis(i / (float)NUM_RAYS * 360, Vector3.forward) * Vector2.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(transform.position, viewConeRadius);

        Vector2[] enemyWeights = CalculateWeights(
                allColliders.Where(c => enemyTags.Contains(c.tag)).Select(x => x.ClosestPoint(transform.position)),
                (currentWeight, index, point) => WeightFunction(currentWeight, index, point, enemyWeightCurve)
            );

        Vector2[] wallWeights = CalculateWeights(
                allColliders.Where(c => wallTags.Contains(c.tag)).Select(x => x.ClosestPoint(transform.position)),
                (currentWeight, index, point) => WeightFunction(currentWeight, index, point, wallWeightCurve)
            );

        Vector2[] pickupWeights = CalculateWeights(
                allColliders.Where(c => pickupTags.Contains(c.tag)).Select(x => x.ClosestPoint(transform.position)),
                (currentWeight, index, point) => WeightFunction(currentWeight, index, point, pickupWeightCurve)
            );

        decisionWeights = CalculateDecisionWeights(enemyWeights, wallWeights, pickupWeights);

        targetVector = CalculateTargetVector();

        for (int i = 0; i < NUM_RAYS; i++)
        {
            Debug.DrawRay(transform.position, enemyWeights[i], Color.red);
            //Debug.DrawRay(transform.position, wallWeights[i], Color.grey);
            Debug.DrawRay(transform.position, pickupWeights[i], Color.blue);
            Debug.DrawRay(transform.position, decisionWeights[i], Color.green);
        }

        Debug.DrawRay(transform.position, targetVector, Color.cyan);

    }

    private Vector2 CalculateTargetVector()
    {
        Vector2 targetVector = Vector2.zero;
        for (int i = 0; i < NUM_RAYS; i++)
        {
            if (decisionWeights[i].sqrMagnitude > targetVector.sqrMagnitude)
            {
                targetVector = decisionWeights[i];
            }
        }
        return targetVector.normalized;
    }

    private Vector2[] CalculateDecisionWeights(Vector2[] enemyWeights, Vector2[] wallWeights, Vector2[] pickupWeights)
    {
        Vector2[] newWeights = new Vector2[NUM_RAYS];
        for (int i = 0; i < NUM_RAYS; i++)
        {
            if (wallWeights[i].magnitude > 0f)
            {
                newWeights[i] = -wallWeights[i];
            }
            if (enemyWeights[i].magnitude > 0f)
            {
                newWeights[i] = -enemyWeights[i];
            }
            if (pickupWeights[i].magnitude > 0f)
            {
                if (pickupWeights[i].sqrMagnitude < enemyWeights[i].sqrMagnitude)
                {
                    newWeights[i] += pickupWeights[i] - enemyWeights[i];
                } else
                {
                    newWeights[i] = Vector2.zero;
                }
            }
        }
        return newWeights;
    }

    private Vector2 WeightFunction(Vector2 currentWeight, int index, Vector3 point, AnimationCurve weightCurve)
    {
        Vector2 dirVector = point - this.transform.position;
        float distanceFactor = 1 - (dirVector.magnitude / viewConeRadius);
        float weightMagnitude = Vector2.Dot(weightDirections[index], dirVector.normalized) * weightCurve.Evaluate(distanceFactor);
        //Debug.Log(distanceFactor);
        //Debug.Log(weightCurve.Evaluate(distanceFactor));
        return weightDirections[index] * Mathf.Max(currentWeight.magnitude, weightMagnitude);
    }

    private Vector2[] CalculateWeights(IEnumerable<Vector2> points, Func<Vector2, int, Vector3, Vector2> weightFunction)
    {
        Vector2[] newWeights = new Vector2[NUM_RAYS];
        foreach (Vector2 point in points)
        {
            for (int j = 0; j < NUM_RAYS; j++)
            {
                newWeights[j] = weightFunction(newWeights[j], j, point);
            }
        }
        return newWeights;
    }

    private void OnDrawGizmos()
    {
        DrawWireSphereForView(Color.white, viewConeRadius);
    }

    private void DrawWireSphereForView(Color c, float viewConeRadius)
    {
        c.a = 0.3f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, viewConeRadius);
    }
}
