using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class GameManager : Singleton<GameManager>
{
    public int score;
    public float normalSpeedMultiplier = 5;
    public float hyperSpeedMultiplier = 40;
    public float normalSpeedTrailLifetimeMultiplier = 0;
    public float hyperSpeedTrailLifetimeMultiplier = 0.04f;
    public List<WaveConfig> waveConfigs;
    public PlayerController playerTemplate;
    public WaveController templateWaveController;
    public TextMeshPro livesText;
    public TextMeshPro scoreText;
    public TextMeshPro gameStatusText;
    public List<Pickable> availablePickables;
    public AudioClip hyperDriveAudioClip;
    public AudioClip explosionAudioClip;
    public AudioClip pickUpAudioClip;

    [System.NonSerialized]
    public float currentSpeed;
    [System.NonSerialized]
    public AudioSource audioSource;
    private PlayerController spawnedPlayer;
    private ParticleSystem backgroundParticleSystem;
    private ParticleSystem.MainModule main;
    private ParticleSystem.TrailModule trails;
    private float? lastPickUpTime;
    protected override void Awake()
    {
        base.Awake();
        backgroundParticleSystem = GetComponent<ParticleSystem>();
        main = backgroundParticleSystem.main;
        trails = backgroundParticleSystem.trails;

        main.startSpeedMultiplier = hyperSpeedMultiplier;
        trails.lifetimeMultiplier = hyperSpeedTrailLifetimeMultiplier;
        trails.enabled = true;
    }

    void Start()
    {
        spawnedPlayer = Instantiate(playerTemplate);

        StartCoroutine(StartWaves());
    }

    void Update()
    {
        if(spawnedPlayer == null || spawnedPlayer.isDead){
            gameStatusText.text = "MISSION FAILED \nPress R to Restart";
            
            if(Input.GetKeyUp(KeyCode.R)){
                SceneManager.LoadScene(0);
            }
        }

        livesText.text = spawnedPlayer.health.ToString();
        scoreText.text = score.ToString();
    }

    private IEnumerator StartWaves()
    {
        // time before spawning waves
        gameStatusText.text = "MISSION:  ELIMINATE  THREATS";
        yield return new WaitForSeconds(2);
        gameStatusText.text = "";

        foreach (WaveConfig waveConfig in waveConfigs)
        {
            if(spawnedPlayer == null || spawnedPlayer.isDead){
                gameStatusText.text = "MISSION  FAILED";
                yield break;
            }

            // time to wait in hyperspeed
            yield return new WaitForSeconds(3);
            audioSource = GetComponent<AudioSource>();
            audioSource.Play();

            // transition to normal speed
            gameStatusText.text = "WARNING:  THREAT  DETECTED\nEXITING  HYPERSPEED...";
            currentSpeed = normalSpeedMultiplier;
            StartCoroutine(StartTransition(3, hyperSpeedMultiplier, normalSpeedMultiplier, hyperSpeedTrailLifetimeMultiplier, normalSpeedTrailLifetimeMultiplier));
            yield return new WaitForSeconds(3);
            trails.enabled = false;
            gameStatusText.text = "";

            // Spawn wave
            templateWaveController.waveConfig = waveConfig;
            WaveController instantiatedWave = Instantiate(templateWaveController);
            yield return new WaitUntil(() => instantiatedWave.isDone);

            // transition to hyperspeed
            audioSource.PlayOneShot(hyperDriveAudioClip);
            gameStatusText.text = "WAVE  CLEARED\nGOING  TO  HYPERSPEED...";
            currentSpeed = hyperSpeedMultiplier;
            trails.enabled = true;
            StartCoroutine(StartTransition(3, normalSpeedMultiplier, hyperSpeedMultiplier, normalSpeedTrailLifetimeMultiplier, hyperSpeedTrailLifetimeMultiplier));
            yield return new WaitForSeconds(3);
            gameStatusText.text = "";
        }
        gameStatusText.text = "MISSION  SUCCESS\nALL  THREATS  ELIMINATED";
    }

    private IEnumerator StartTransition(
        float transitionTime,
        float fromSpeed,
        float toSpeed,
        float fromTrailSpeed,
        float toTrailSpeed)
    {
        float t = 0;
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[backgroundParticleSystem.main.maxParticles];
        while (t < 1)
        {
            t += Time.deltaTime / transitionTime;
            SetParticleSpeed(Vector3.left * Mathf.Lerp(fromSpeed, toSpeed, t), backgroundParticleSystem, m_Particles);
            trails.lifetimeMultiplier = Mathf.Lerp(fromTrailSpeed, toTrailSpeed, t);
            yield return new WaitForEndOfFrame();
        }
        SetParticleSpeed(Vector3.left * toSpeed, backgroundParticleSystem, m_Particles);
        main.startSpeedMultiplier = toSpeed;
        trails.lifetimeMultiplier = toTrailSpeed;
    }

    private static void SetParticleSpeed(Vector3 velocity, ParticleSystem backgroundParticleSystem, ParticleSystem.Particle[] m_Particles)
    {
        int numParticlesAlive = backgroundParticleSystem.GetParticles(m_Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            m_Particles[i].velocity = velocity;
        }
        backgroundParticleSystem.SetParticles(m_Particles, numParticlesAlive);
    }

    public void OnKill(int increment, Vector2 killLocation)
    {
        score += increment;

        if ((score > 500 && lastPickUpTime == null) || (Time.time - lastPickUpTime > 20))
        {
            Pickable chosen = availablePickables[Random.Range(0, availablePickables.Count)];
            // Pickable chosen = availablePickables[1];
            Pickable instantiated = Instantiate(chosen, killLocation, Quaternion.identity);
            instantiated.SetSpeed(currentSpeed);
            lastPickUpTime = Time.time;
        }
    }
}