using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RubyController : MonoBehaviour
{
    public int maxHealth = 5;
    public float speed = 3.0f;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; } }
    int currentHealth;
    bool isInvincible;
    float invincibleTimer;

    Animator animator;
    Vector2 lookDirection = new Vector2(0, -1);

    Rigidbody2D rigidbody2d;
    AudioSource audioSource;

    public GameObject projectilePrefab;
    public ParticleSystem hitParticle;
    public ParticleSystem pickupEffect;
    public Transform respawnPosition;
    public AudioClip hitSound;
    public AudioClip shootingSound;

    public TextMeshProUGUI tdetails;
    public int Details = 0;
    public int CopyDetails = 0;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                NPC1 character1 = hit.collider.GetComponent<NPC1>();
                NPC2 character2 = hit.collider.GetComponent<NPC2>();
                if (character != null)
                {
                    if (CopyDetails == 0)
                    {
                        character.DisplayDialog();
                    }
                    if (CopyDetails != 0 && CopyDetails <= 9)
                    {
                        character1.DisplayDialog();
                    }
                    if (Details >= 10)
                    {
                        character2.DisplayDialog();
                        Details = Details - 10;
                    }
                    if (CopyDetails >= 10)
                    {
                        character2.DisplayDialog();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Exit();
        }
        SetCountText();
    }
    void Respawn()
    {
        currentHealth = maxHealth;
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        transform.position = respawnPosition.position;
    }
    void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    public void ChangeHealth(int amount)
    {
        if (currentHealth == 0)
        {
            Respawn();
        }
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);
            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        if (amount > 0)
        {
            Instantiate(pickupEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);
        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(shootingSound);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    void SetCountText()
    {
        tdetails.text = Details.ToString();
    }
}