using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityStates : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class Timer<T> : State where T : MonoStateMachine
    {

        public T value;
        public float m_Timer = 0;
        public Action TimerFinshed;
        //public delegate void IsTimerFinshed();

        /// <summary>
        /// Timer Constructor
        /// </summary>
        /// <param name="t">Statemachine Object</param>
        /// <param name="TimeSet">Time in Seconds to count for</param>
        /// <param name="name">Name of the State</param>
        public Timer(T t,float TimeSet,string name)
        {
            Name = name;
            value = t; 
            m_Timer = TimeSet;
        }

        public override void In()
        {
            //throw new System.NotImplementedException();
        }

        public override void Out()
        {
            TimerFinshed();
            value.StateTable.Remove(this);
            //throw new System.NotImplementedException();
        }

        public override void Tick()
        {
            if (m_Timer > 0) 
            { 
                m_Timer -= (1 * Time.deltaTime); 
            }
            else
            {                
                Services.Resolve<GenericStateEngine>().ExitState(value.MachineId,Name, false);
            }
        }
    }

    public class ConditionalCheck : State
    {
        public override void In()
        {
            //throw new NotImplementedException();
        }

        public override void Out()
        {
            //throw new NotImplementedException();
        }

        public override void Tick()
        {
            //throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Generic State
    /// </summary>
    public class GenericState : State
    {

        public Action IN, OUT, TICK;

        public override void In()
        {
            IN();
        }

        public override void Out()
        {
            OUT();
        }

        public override void Tick()
        {
            TICK();
        }
    }

}


