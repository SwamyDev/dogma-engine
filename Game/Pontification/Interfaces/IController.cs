using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pontification.AI;

namespace Pontification.Interfaces
{
    public interface IController
    {
        AIMemorySheet Memory { get; }

        void SetTarget(GameObject go);
        void Attack();
        void Walk(int direction);
        void Jump();
        void ChangeFacing(int direction);
    }
}
