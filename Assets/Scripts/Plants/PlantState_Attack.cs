using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plant
{
    public class PlantState_Attack : IState
    {
        Animator animator;

        AudioSource throwSound;
        Coroutine fireCoroutine;
        GameObject bullet;
        AudioClip[] throwSoundClips = null;
        PlantBase plant;

        GameObject bullets;

        float throwCD = 2f;
        public PlantState_Attack(PlantBase plant, Animator animator)
        {
            this.animator = animator;
            this.plant = plant;
            bullet = plant.bullet;
            throwSoundClips = plant.throwSoundClips;
            throwSound = plant.throwSound;
            bullets = GameObject.Find("Bullets");
        }
        public void OnEnter()
        {
            animator.SetBool("isAttacking", true);
            fireCoroutine = plant.StartCoroutine(ThrowBullet());
        }

        public void OnLeave()
        {
            plant.StopCoroutine(fireCoroutine);
            animator.SetBool("isAttacking", false);
        }

        public void OnState()
        {
            RaycastHit2D hit;
            LayerMask mask = 1 << 6;
            hit = Physics2D.Raycast(plant.transform.position, plant.transform.right, 20, mask);
            if (!hit)
            {
                plant.TransitionState(Plant.PlantBase.PlantState.Ilde);
            }
            else
            {
                Debug.DrawLine(plant.transform.position, plant.transform.position + plant.transform.right * 10, Color.red);
            }
        }

        IEnumerator ThrowBullet()
        {
            while (true)
            {
                if (throwSound != null)
                {
                    int index = Random.Range(0, throwSoundClips.Length);
                    throwSound.clip = throwSoundClips[index];
                    throwSound.Play();
                }
                else
                {
                    Debug.LogError("ThrowSoundList is empty");
                }

                Object.Instantiate(bullet, plant.transform.position, plant.transform.rotation, parent: bullets.transform);
                yield return new WaitForSeconds(throwCD);
            }
        }
    }
}