using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plant
{
    public class PlantBase : MonoBehaviour
    {
        public enum PlantState
        {
            Ilde,
            Attack
        }
        protected HealthComp healthComp = null;
        protected Animator animator;
        [SerializeField] protected Dictionary<PlantState, IState> states = new();
        [SerializeField] protected PlantState state;

        public GameObject bullet = null;
        public AudioClip[] throwSoundClips = null;
        public AudioSource throwSound;


        public void Start()
        {
            healthComp = GetComponent<HealthComp>();
            animator = GetComponent<Animator>();
            throwSound = GetComponent<AudioSource>();

            states.Add(PlantState.Ilde, new PlantState_Idle(this, 20));
            states.Add(PlantState.Attack, new PlantState_Attack(this, animator));
            state = PlantState.Ilde;
        }

        public void TakeDamage(float damage)
        {
            healthComp.TakeDamage(damage);
        }

        public virtual void Attack()
        {
        }

        public void TransitionState(PlantState newState)
        {
            states[state]?.OnLeave();
            state = newState;
            states[state].OnEnter();
        }
    }
}