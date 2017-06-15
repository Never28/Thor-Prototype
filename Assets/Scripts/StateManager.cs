using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager {

	public State curState;
	public State prevState;
    public State defaultState;
    public List<State> states;
	PlayerController player;

	public void Update(){
        for (int i = 0; i < states.Count - 1; i++) {
            if (states[i] != curState) {
                if (states[i].EnterConditions(player, curState))
                {
                    ChangeCurrentState(states[i]);
                    break;
                }
            }
        }
		if (curState != null) {
			curState.Update (player);
		}
	}

	public void ChangeCurrentState(State nextState){

		if (curState != null) {
			curState.Exit (player);
		}
		prevState = curState;
		curState = nextState;
		if (curState != null) {
			curState.Enter (player);
		}
	}
}
