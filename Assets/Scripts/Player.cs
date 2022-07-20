using UnityEngine;

public class Player : MonoBehaviour
{
    public int Score { get; set; }
    public int GemCount { get; set; }
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        gameManager.HandleTrigger(gameObject, other.gameObject);
    }
}
