using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    private bool isFinished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player") && !isFinished)
        {
            isFinished = true;

            
            Debug.Log("Goal reached!");

            
            if (GameManager.instance != null)
            {
                GameManager.instance.FinishGame();
            }
        }
    }
}