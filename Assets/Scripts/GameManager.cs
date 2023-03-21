using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player player;
    public ParticleSystem explosionEffect;
    public GameObject gameOverUI;
    public AsteroidSpawner spawn;

    public int score { get; private set; }
    public Text scoreText;

    public int lives { get; private set; }
    public Text livesText;

    public bool isBoosterOnScreen;

    private void Start()
    {
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    public void NewGame()
    {

        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(player.playerdata.playerHealth);
        Respawn();
        
    }

    public void Respawn()
    {
        player.transform.position = Vector3.zero;
        player.gameObject.SetActive(true);
    }

    public void AsteroidDestroyed(Asteroid asteroid)
    {
        explosionEffect.transform.position = asteroid.transform.position;
        explosionEffect.Play();

        if (asteroid.size < 0.7f) {
            SetScore(score + 100); // small asteroid
        } 
    }

    public void PlayerDeath(Player player)
    {
        explosionEffect.transform.position = player.transform.position;
        explosionEffect.Play();
        GameOver();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();

        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }
    }

    private void SetScore(int score)
    {
        this.score = score;
        if (player.playerdata.scoreForIncreasingDifficulty < 100)
            player.playerdata.scoreForIncreasingDifficulty = 100;

        if (score%player.playerdata.scoreForIncreasingDifficulty==0)
        {
            //Debug.Log("Increase");
            spawn.amountPerSpawn++;
            spawn.StartSpawning();
        }
        scoreText.text = score.ToString();
    }

    public void SetLives(int lives)
    {
        this.lives = lives;
        livesText.text = lives.ToString();
    }

}
