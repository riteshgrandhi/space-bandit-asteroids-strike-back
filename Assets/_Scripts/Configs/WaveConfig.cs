using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Configs/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [SerializeField]
    public List<Phase> phases;
}

[Serializable]
public class Phase
{
    public Enemy enemy;
    [Range(1, 10)]
    public int count;

    [Range(0, 6)]
    public float maxWaitBeforeEachSpawnSeconds = 0.5f;
    public OnPhaseSpawningDone onPhaseSpawningDone;
    public float waitBeforeNextPhaseSeconds = 2;
}

public enum OnPhaseSpawningDone
{
    CONTINUE_TO_NEXT_PHASE,
    // WAIT_FOR_THIS_PHASE_DONE,
    WAIT_FOR_ALL_CURRENT_DONE
}