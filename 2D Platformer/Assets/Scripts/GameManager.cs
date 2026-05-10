using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Elements")]
    public GameObject endScreen; 
    public TextMeshProUGUI resultText; 

    private float timer;
    private bool isFinished = false;

    void Awake()
    {
        
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        
        if (!isFinished)
        {
            timer += Time.deltaTime;
        }

        
        if (isFinished && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void FinishGame()
    {
        if (isFinished) return;
        isFinished = true;

        
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        
        if (endScreen != null) endScreen.SetActive(true);

        
        if (resultText != null)
        {
            resultText.text = "CONGRATULATIONS!\n" +
                              "YOU FINISHED THE ASPEN\n\n" +
                              "FINAL TIME: " + string.Format("{0:00}:{1:00}", minutes, seconds) + "\n\n" +
                              "<size=60%>Press 'R' to Restart</size>";
        }

        
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            
            player.enabled = false;

            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }

            
            Animator anim = player.GetComponent<Animator>();
            if (anim != null) anim.Play("Player_Idle");
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}