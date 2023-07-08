using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    private static int NUM_RAYS = 8;
    public float interestViewConeRadius = 15;
    public float dangerViewConeRadius = 6;
    public Vector2 targetVector { get; private set; }

    private int interestLayers;
    private int dangerLayers;

    Vector2[] interestWeights = new Vector2[NUM_RAYS];
    Vector2[] dangerWeights = new Vector2[NUM_RAYS];
    Vector2[] decisionWeights = new Vector2[NUM_RAYS];
    Vector2[] weightDirections = new Vector2[NUM_RAYS];

    void Awake()
    {
        dangerLayers = LayerMask.GetMask("Enemy", "Wall");
        interestLayers = LayerMask.GetMask("Pickup");


        for (int i = 0; i < NUM_RAYS; i++)
        {
            weightDirections[i] = Quaternion.AngleAxis(i / (float)NUM_RAYS * 360, Vector3.forward) * Vector2.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        dangerWeights = CalculateWeights(
                Physics2D.OverlapCircleAll(transform.position, dangerViewConeRadius, dangerLayers).Select(x => x.ClosestPoint(transform.position)),
                (currentWeight, index, point) => DangerWeightFunction(currentWeight, index, point)
            );

        interestWeights = CalculateWeights(
                Physics2D.OverlapCircleAll(transform.position, interestViewConeRadius, interestLayers).Select(x => x.ClosestPoint(transform.position)),
                (currentWeight, index, point) => InterestWeightFunction(currentWeight, index, point)
            );

        decisionWeights = CalculateDecisionWeights();

        targetVector = CalculateTargetVector();

        for (int i = 0; i < NUM_RAYS; i++)
        {
            Debug.DrawRay(transform.position, dangerWeights[i], Color.red);
            Debug.DrawRay(transform.position, interestWeights[i], Color.green);
            Debug.DrawRay(transform.position, decisionWeights[i], Color.blue);
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

    private Vector2[] CalculateDecisionWeights()
    {
        Vector2[] newWeights = new Vector2[NUM_RAYS];
        for (int i = 0; i < NUM_RAYS; i++)
        {
            if (dangerWeights[i].magnitude > 0.9f)
            {
                newWeights[i] = -dangerWeights[i];
            }
            else if (dangerWeights[i].magnitude > 0.4f)
            {
                newWeights[i] = Vector2.zero;
            }
            else
            {
                newWeights[i] = interestWeights[i] - dangerWeights[i];
            }
        }
        return newWeights;
    }

    private Vector2 DangerWeightFunction(Vector2 currentWeight, int index, Vector3 point)
    {
        Vector2 dirVector = point - this.transform.position;
        return weightDirections[index] * Mathf.Max(currentWeight.magnitude, Vector2.Dot(weightDirections[index], (dirVector.normalized * dangerViewConeRadius / dirVector.magnitude)));
    }

    private Vector2 InterestWeightFunction(Vector2 currentWeight, int index, Vector3 point)
    {
        Vector2 dirVector = point - this.transform.position;
        return weightDirections[index] * Mathf.Max(currentWeight.magnitude, Vector2.Dot(weightDirections[index], (dirVector.normalized * interestViewConeRadius / dirVector.magnitude)));
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
        DrawWireSphereForView(Color.red, dangerViewConeRadius);
        DrawWireSphereForView(Color.green, interestViewConeRadius);
    }

    private void DrawWireSphereForView(Color c, float viewConeRadius)
    {
        c.a = 0.1f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, viewConeRadius);
    }
}
