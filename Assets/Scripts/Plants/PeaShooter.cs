using Plant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plant
{
    public class PeaShooter : PlantBase
    {

        private void Update()
        {
            states[state].OnState();
        }
        // Update is called once per frame
        //public void DetectAndProject()
        //{
        //    RaycastHit2D hit;
        //    LayerMask mask = 1 << 6;
        //    hit = Physics2D.Raycast(transform.position, transform.right, detectDistance, mask);
        //    if (hit)
        //    {
        //        Debug.DrawLine(transform.position, hit.point, Color.red);
        //        if (state != PlantState.Attack)
        //        {
        //            TransitionState(PlantState.Attack);
        //        }
        //    }
        //    else
        //    {
        //        Debug.DrawLine(transform.position, transform.position + transform.right * 10, Color.blue);
        //        if (state == PlantState.Attack)TransitionState(PlantState.Ilde);
        //        if (fireTrigger != null) StopCoroutine(fireTrigger);
        //    }
        //}

    }
}