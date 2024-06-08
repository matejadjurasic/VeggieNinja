using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text scoreTextFinish;
    private int score;
    private Blade blade;
    private Spawner spawner;
    public Image fadeImage;
    public Image[] veggieImages;
    private int currentHealth;
    public Sprite fullHealthSprite;
    public Sprite halfHealthSprite;
    public Sprite emptyHealthSprite;
    public GameObject gameOverMenu;
    public AudioSource gameStartAudioSource;
    public AudioSource gameEndAudioSource;

    private void Awake()
    {
        blade = FindObjectOfType<Blade>();
        spawner = FindObjectOfType<Spawner>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        gameStartAudioSource.Play();
        Time.timeScale = 1f;
        blade.enabled = true;
        spawner.enabled = true;
        score = 0;
        scoreText.text = score.ToString();
        scoreTextFinish.text = score.ToString() + " POINTS";;
        currentHealth = veggieImages.Length * 2;
        gameOverMenu.SetActive(false);
        for (int i = 0; i < veggieImages.Length; i++)
        {
            veggieImages[i].sprite = fullHealthSprite;
        }
        ClearScene();
    }

    private void ClearScene()
    {
        Veggies[] veggies = FindObjectsOfType<Veggies>();

        foreach (Veggies veggie in veggies)
        {
            Destroy(veggie.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
        scoreTextFinish.text = score.ToString() + " POINTS";
    }

    public void LoseHealth()
    {
        currentHealth -= 1;
        if (currentHealth <= 0)
        {
            UpdateHealthUI();
            Explode();
            GameOver();
        }
        else
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < veggieImages.Length; i++)
        {
            if (currentHealth >= (i + 1) * 2)
            {
                veggieImages[i].sprite = fullHealthSprite;
            }
            else if (currentHealth == (i * 2) + 1)
            {
                veggieImages[i].sprite = halfHealthSprite;
            }
            else
            {
                veggieImages[i].sprite = emptyHealthSprite;
            }
        }
    }

    private void GameOver()
    {
        gameEndAudioSource.Play();
        blade.enabled = false;
        spawner.enabled = false;
        Time.timeScale = 0f;
        int highScore = HighScoreManager.GetHighScore();

        if (score > highScore)
        {
            HighScoreManager.SetHighScore(score);
        }

        gameOverMenu.SetActive(true);
    }

    public void RestartGame()
    {
        NewGame();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);
            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
        GameOver();
    }

    public int GetScore()
    {
        return score;
    }
}
