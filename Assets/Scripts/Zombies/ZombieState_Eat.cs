using UnityEngine;
public class ZombieState_Eat : IState
{
    private ZombieBase zombie;
    HealthComp plant;


    public ZombieState_Eat(ZombieBase zombie, HealthComp plant)
    {
        this.zombie = zombie;
        this.plant = plant;
    }
    public void OnEnter()
    {
        zombie.rb.velocity = Vector2.zero;
        Debug.Log("Stop!");
        return;
    }

    public void OnLeave()
    {
        return;
    }

    public void OnState()
    {
        plant.TakeDamage(zombie.attack);
        Debug.Log("WowEat"+ plant.gameObject.name);
        return;
    }
}
