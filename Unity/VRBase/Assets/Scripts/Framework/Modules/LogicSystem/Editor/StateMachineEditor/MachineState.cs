﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Game.LogicSystem.Editor
{
    public class MachineState
    {
        public int id;
        public List<StateTransition> stateTransitions = new List<StateTransition>();

        public List<StateBaseBehaviour> stateBaseBehaviours = new List<StateBaseBehaviour>();
    }
}

