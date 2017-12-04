using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HDJ.Framework.Game.LogicSystem.Editor
{
    [SerializeField]
    public class StateTransition
    {
        public MachineState fromState;
        public MachineState toState;

        public StateTransition() { }
        public StateTransition(MachineState fromState, MachineState toState)
        {
            this.fromState = fromState;
            this.toState = toState;
        }
    }
}