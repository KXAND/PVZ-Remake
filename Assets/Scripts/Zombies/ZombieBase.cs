using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zombie
{
    enum ZombieState
    {
        Idle,
        Eat,
        Walk
    }

    public class ZombieBase : MonoBehaviour
    {
        public float speed;
        public Rigidbody2D rb;
        public AudioSource audioSource;
        public Animator animator;
        public float attack = 10f;


        HealthComp healthComp;

        Dictionary<ZombieState, IState> states = new();
        [SerializeField] ZombieState state;

        [SerializeField] SoundPack soundPack;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            healthComp = gameObject.AddComponent<HealthComp>();

            states.Add(ZombieState.Idle, new ZombieState_Idle(this));
            states.Add(ZombieState.Walk, new ZombieState_Walk(this, audioSource, soundPack.groanSounds));
            states.Add(ZombieState.Eat, new ZombieState_Eat(this));

            TransitionState(ZombieState.Walk);
            healthComp.Init(100);
        }

        private void Update()
        {
            states[state]?.OnState();
        }

        void TransitionState(ZombieState newState)
        {
            states[state]?.OnLeave();
            state = newState;
            states[newState].OnEnter();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                TransitionState(ZombieState.Eat);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Plant"))
            {
                TransitionState(ZombieState.Walk);
            }
        }
    }
}