using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttacking : IState
{
    public bool CanEnter(IState currentState)
    {
        return KnightFSMController.instance.IsEnemyInRangeAndAttack();
    }

    public bool CanExit()
    {
        return KnightFSMController.instance.NoEnemyInRange();
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Update()
    {
        KnightFSMController.instance.Attack();
    }
}
