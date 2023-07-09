using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class GameManager : Singleton<GameManager>
{
    public int score;
    public WaveConfig[] waveConfigs;
    public WaveController templateWaveController;
    public TextMeshPro livesText;
    public TextMeshPro scoreText;
    public TextMeshPro gameStatusText;
    public Pickable[] availablePickables;
    public AudioClip hyperDriveAudioClip;
    public AudioClip explosionAudioClip;
    public AudioClip pickUpAudioClip;

    [System.NonSerialized]
    public float currentSpeed;
    [System.NonSerialized]
    public AudioSource audioSource;
    public PlayerController spawnedPlayer;
    private Damageable spawnedPlayerDamageable;
    private float? lastPickUpTime;

    void Start()
    {
        spawnedPlayerDamageable = spawnedPlayer.GetComponent<Damageable>();
    }

    void Update()
    {
        if (spawnedPlayer == null || spawnedPlayerDamageable.isDead)
        {
            gameStatusText.text = "MISSION FAILED \nPress R to Restart";

            if (Input.GetKeyUp(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }

        livesText.text = spawnedPlayerDamageable.health.ToString();
        scoreText.text = score.ToString();
    }

    public void OnKill(int increment, Vector2 killLocation)
    {
        score += increment;

        if ((score > 500 && lastPickUpTime == null) || (Time.time - lastPickUpTime > 20))
        {
            Pickable chosen = availablePickables[Random.Range(0, availablePickables.Length)];
            // Pickable chosen = availablePickables[1];
            Pickable instantiated = Instantiate(chosen, killLocation, Quaternion.identity);
            instantiated.SetSpeed(currentSpeed);
            lastPickUpTime = Time.time;
        }
    }
}