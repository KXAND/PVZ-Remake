using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectPea : MonoBehaviour
{
    readonly float speed = 0.1f;
    bool canCollide = true;

    [SerializeField] float damage = 10f;
    [SerializeField] Sprite[] splatSprites;

    void FixedUpdate()
    {
        transform.position += Vector3.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && canCollide)
        {
            Zombie.ZombieBase zombie = collision.GetComponent<Zombie.ZombieBase>();
            if (zombie) zombie.TakeDamage(damage);

            int index = Random.Range(0, splatSprites.Length);
            GetComponent<SpriteRenderer>().sprite = splatSprites[index];
            transform.GetChild(1).gameObject.SetActive(true);
            canCollide = false;
            StartCoroutine(WaitAndDestroy());
        }
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
