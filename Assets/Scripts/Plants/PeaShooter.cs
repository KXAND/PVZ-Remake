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
    }
}