using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieBase : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;
    public float attack = 10f;

    HealthComp healthComp;

    IState state;
    public void Init()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        TransitionState(new ZombieState_Walk(this));
        healthComp = gameObject.AddComponent<HealthComp>();
        healthComp.Init(100);
    }

    private void Update()
    {
        state?.OnState();
    }

    void TransitionState(IState newState)
    {
        state?.OnLeave();
        state = newState;
        state.OnEnter();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Plant"))
        {

            TransitionState(new ZombieState_Eat(this, collision.GetComponent<HealthComp>()));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Plant"))
        {
            TransitionState(new ZombieState_Walk(this));
        }
    }
}
