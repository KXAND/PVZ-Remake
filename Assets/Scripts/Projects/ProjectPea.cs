using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectsPea : MonoBehaviour
{
    readonly float speed = 0.1f;

    [SerializeField] float damage = 10f;
    [SerializeField] AudioSource splatSound = null;
    [SerializeField] SpriteRenderer peaSplat = null;
    [SerializeField] Sprite[] splatSprites;

    void FixedUpdate()
    {
        transform.position += Vector3.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            HealthComp zombie = collision.GetComponent<HealthComp>();
            if (zombie) zombie.TakeDamage(damage);

            Instantiate(peaSplat, transform.position, Quaternion.identity);
            int index = Random.Range(0, splatSprites.Length);
            peaSplat.sprite = splatSprites[index];
            ParticleSystem particleSystem = peaSplat.GetComponentInChildren<ParticleSystem>();
            particleSystem.Play();
            if (splatSound) splatSound.Play();
            Debug.Log(particleSystem.gameObject.name);
            Destroy(gameObject);

        }
    }
}
