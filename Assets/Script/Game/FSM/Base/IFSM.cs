using UnityEngine;
using System.Collections;


namespace FSM
{
    public interface IFSM
    {
        void Init();
        void Enter();
        void Leave();
        void Update();
        void Destroy();

    }
}

