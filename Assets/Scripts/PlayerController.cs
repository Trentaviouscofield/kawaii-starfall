using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float horizontalLimit = 8f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float baseFireRate = 0.3f;
    [SerializeField] private float bulletSpeed = 12f;

    private float nextFireTime;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.IsGameOver)
        {
            return;
        }

        HandleMovement();
        if (!GameManager.Instance.IsGameStarted) return;
        HandleShooting();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector3 position = transform.position;
        position.x += moveInput * moveSpeed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -horizontalLimit, horizontalLimit);
        transform.position = position;
    }

    private void HandleShooting()
    {
        if (!Input.GetKey(KeyCode.Space)) return;
        if (Time.time < nextFireTime) return;

        int weaponLevel = GameManager.Instance.WeaponLevel;
        float fireRateModifier = weaponLevel >= 4 ? 0.75f : 1f;
        nextFireTime = Time.time + (baseFireRate * fireRateModifier);

        FirePattern(weaponLevel);
    }

    private void FirePattern(int level)
    {
        switch (level)
        {
            case 1:
                SpawnBullet(0f);
                break;
            case 2:
                SpawnBullet(-0.15f, -0.4f);
                SpawnBullet(0.15f, 0.4f);
                break;
            case 3:
                SpawnBullet(0f);
                SpawnBullet(-0.2f, -12f);
                SpawnBullet(0.2f, 12f);
                break;
            case 4:
                SpawnBullet(0f, 0f, bulletSpeed * 1.25f);
                SpawnBullet(-0.2f, -12f, bulletSpeed * 1.25f);
                SpawnBullet(0.2f, 12f, bulletSpeed * 1.25f);
                break;
            default:
                SpawnBullet(-0.35f, -20f, bulletSpeed * 1.3f);
                SpawnBullet(-0.2f, -10f, bulletSpeed * 1.3f);
                SpawnBullet(0f, 0f, bulletSpeed * 1.3f);
                SpawnBullet(0.2f, 10f, bulletSpeed * 1.3f);
                SpawnBullet(0.35f, 20f, bulletSpeed * 1.3f);
                break;
        }
    }

    private void SpawnBullet(float xOffset = 0f, float angle = 0f, float speedOverride = -1f)
    {
        Vector3 spawnPos = firePoint.position + new Vector3(xOffset, 0f, 0f);
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        float finalSpeed = speedOverride > 0f ? speedOverride : bulletSpeed;
        Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        bullet.Initialize(direction, finalSpeed);
    }
}
