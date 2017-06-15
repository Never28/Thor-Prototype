using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class State {

    virtual public void Update(PlayerController player) { 
    }

    virtual public void Enter(PlayerController player)
    { 
    }

    virtual public void Exit(PlayerController player)
    { 
    }

    virtual public bool EnterConditions(PlayerController player, State curState)
    {
        return false;
    }
}

public class Moving : State
{
    public void Update(PlayerController player) {

    }

    public void Enter(PlayerController player) { 
    }

    public void Exit(PlayerController player) { 
    }

    public State ExitConditions(PlayerController player, State curState) {
        return null;
    }
}

public class Idle : State
{
    public void Update(PlayerController player)
    {

    }

    public void Enter(PlayerController player)
    {
    }

    public void Exit(PlayerController player)
    {
    }

    public State ExitConditions(PlayerController player, State curState)
    {
        return null;
    }
}

 