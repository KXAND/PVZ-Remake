using UnityEngine;
public class ZombieState_Walk : IState
{
    ZombieBase zombie;
    //float speed = 0;
    public ZombieState_Walk(ZombieBase zombie) { this.zombie = zombie; }
    public void OnEnter()
    {
        // 实现行走动画
        zombie.rb.velocity = new Vector2(-1 * zombie.speed, 0);
        return;
    }

    public void OnLeave()
    {
        return;
    }

    public void OnState()
    {
        return;
    }


}
