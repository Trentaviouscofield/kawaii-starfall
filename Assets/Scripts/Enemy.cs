using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private float upgradeDropChance = 0.15f;
    [SerializeField] private GameObject upgradeOrbPrefab;

    private float speed;
    private int currentHealth;
    private float bottomYLimit = -6f;

    public void Initialize(float moveSpeed, float bottomLimit)
    {
        speed = moveSpeed;
        bottomYLimit = bottomLimit;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        transform.Translate(Vector3.down * (speed * Time.deltaTime));

        if (transform.position.y <= bottomYLimit)
        {
            GameManager.Instance.LoseHeart();
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(scoreValue);

        if (upgradeOrbPrefab != null && Random.value <= upgradeDropChance)
        {
            Instantiate(upgradeOrbPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
