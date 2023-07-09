using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public bool isDone = false;
    public WaveConfig waveConfig;
    private List<Damageable> allSpawnedEnemies = new List<Damageable>();
    private List<Damageable> currentPhaseEnemies = new List<Damageable>();
    void Start() => StartCoroutine(StartWavePhases());

    private IEnumerator StartWavePhases()
    {
        foreach (Phase currentPhase in waveConfig.phases)
        {
            currentPhaseEnemies.Clear();
            for (int i = 0; i < currentPhase.count; i++)
            {
                Vector2 spawnPosition = new Vector3(10, Random.Range(7, -7));
                Enemy spawnedEnemy = Instantiate(currentPhase.enemy, spawnPosition, Quaternion.identity);
                
                yield return new WaitForSeconds(Random.Range(0, currentPhase.maxWaitBeforeEachSpawnSeconds));
                // spawnedEnemy.SetSpeed(GameManager.Instance.currentSpeed);

                Damageable spawnedEnemyDamageable = spawnedEnemy.GetComponent<Damageable>();
                spawnedEnemyDamageable.OnDeathHandler += OnSpawnedEnemyDeath;
                currentPhaseEnemies.Add(spawnedEnemyDamageable);
                allSpawnedEnemies.Add(spawnedEnemyDamageable);
            }

            switch (currentPhase.onPhaseSpawningDone)
            {
                case OnPhaseSpawningDone.WAIT_FOR_ALL_CURRENT_DONE:
                    yield return new WaitUntil(() => allSpawnedEnemies.Count == 0);
                    break;
                    // Currently not working as expected
                // case OnPhaseSpawningDone.WAIT_FOR_THIS_PHASE_DONE:
                    // yield return new WaitUntil(() => currentPhaseEnemies.Count == 0);
                    // break;
                case OnPhaseSpawningDone.CONTINUE_TO_NEXT_PHASE:
                default:
                    break;
            }

            yield return new WaitForSeconds(currentPhase.waitBeforeNextPhaseSeconds);
        }
        isDone = true;
    }

    private void OnSpawnedEnemyDeath(Damageable enemy)
    {
        bool resultAll = allSpawnedEnemies.Remove(enemy);

        bool resultCurrent = currentPhaseEnemies.Remove(enemy);
    }
}