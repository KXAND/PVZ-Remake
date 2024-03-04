using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plant
{
    public class PlantState_Idle : IState
    {
        float detectDistance = 0f;
        PlantBase plant;
        Transform transform;
        public PlantState_Idle(PlantBase plant, float detectDistance)
        {
            this.plant = plant;
            transform = plant.transform;
            this.detectDistance = detectDistance;
        }
        public void OnEnter()
        {

        }

        public void OnLeave()
        {
            
        }

        public void OnState()
        {
            RaycastHit2D hit;
            LayerMask mask = 1 << 6;
            hit = Physics2D.Raycast(transform.position, transform.right, detectDistance, mask);
            if (hit)
            {
                plant.TransitionState(Plant.PlantBase.PlantState.Attack);
            }
            else
            {
                Debug.DrawLine(transform.position, transform.position + transform.right * 10, Color.blue);
            }
        }
    }
}