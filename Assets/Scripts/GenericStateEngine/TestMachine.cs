using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

//Oliver.T 06-06-2024
public class TestMachine : MonoStateMachine
{

    public List<GameObject> Cubes = new List<GameObject>();

    private void Start()
    { 
        BuildStateTable();
        Services.Resolve<GenericStateEngine>().LoadStateMachine(this);
    }

    /// <summary>
    /// Construct the interals of the state machine
    /// </summary>
    public override void BuildStateTable()
    {
        var BeginState = new Begin(this,"Begin");
        var Timer = new UtilityStates.Timer<TestMachine>(this, UnityEngine.Random.Range(1, 20), "Timer_1");
        var MiddleState = new Middle(this,"Middle");
        var EndState = new End(this,"End");

        BeginState.NextStates.Add(MiddleState);

        Timer.TimerFinshed += () =>
        {         
            foreach (var Target in Cubes)
            {
                Target.GetComponent<Renderer>().material.color = Color.red;
            }
        };

        MiddleState.NextStates.Add(EndState);
        EndState.NextStates.Add(BeginState);

        ActiveState = BeginState; //could also be ActiveState.Add(BeginState)
        ActiveStates.Add(Timer);

        StateTable.Add(Timer);
        StateTable.Add(BeginState);
        StateTable.Add(MiddleState);
        StateTable.Add(EndState);
    }

    internal class Begin : State
    {
        TestMachine machine;
        public int Count = 0;
        public GameObject Cube;


        public Begin(TestMachine Machine,string name)
        {
            machine = Machine;      
            Name = name;
            Cube = machine.Cubes[0];
        }

        public override void In()
        {
            Cube.GetComponent<Renderer>().material.color = Color.red;
            //Debug.Log(string.Format("Entering {0} in FSM {1}", MethodInfo.GetCurrentMethod(), machine.MachineId));
            //Debug.Log(string.Format("My Type is {0}",GetType().ToString()));
        }

        public override void Out()
        {
            Cube.GetComponent<Renderer>().material.color = Color.gray;
            // Debug.Log(string.Format("My Type is {0}", GetType().ToString()));
            // Debug.Log(string.Format("Leaveing {0} in FSM {1}", MethodInfo.GetCurrentMethod(), machine.MachineId));
        }

        public override void Tick()
        {
            Count++;
            Cube.GetComponent<Renderer>().material.color = Color.blue;
            if (Count > 1000)
            {
                Count = 0;             
                //Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId,StateChange.Next,Name);
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId,"End",Name);
            }
        }
    }

    internal class Middle : State
    {
        TestMachine machine;
        public int Count = 0;
        public GameObject Cube;
        public Middle(TestMachine Machine, string name)
        {
            machine = Machine;
            Name = name;
            Cube = machine.Cubes[1];
        }

        public override void In()
        {
            Cube.GetComponent<Renderer>().material.color = Color.red;
            //machine.ActiveState = this;
            //Debug.Log(string.Format("Entering {0} in FSM {1}", MethodInfo.GetCurrentMethod(), machine.MachineId));
           //Debug.Log(string.Format("My Type is {0}", GetType().ToString()));
        }

        public override void Out()
        {
            Cube.GetComponent<Renderer>().material.color = Color.gray;
            // Debug.Log(string.Format("Leaveing {0} in FSM {1}", MethodInfo.GetCurrentMethod(), machine.MachineId));
        }

        public override void Tick()
        {
            Count++;
            Cube.GetComponent<Renderer>().material.color = Color.blue;
            if (Count > 1000)
            {
                Count = 0;
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId, StateChange.Next, Name);
            }
        }
    }

    internal class End : State
    {
        TestMachine machine;
        public int Count = 0;
        GameObject Cube;

        public End(TestMachine Machine,string name)
        {
            machine = Machine;
            Name = name;
            Cube= machine.Cubes[2];
        }

        public override void In()
        {
            Cube.GetComponent<Renderer>().material.color = Color.red;
        }

        public override void Out()
        {
            if (machine.SearchStateTable("TimerEnd"))
            {
                var NewTimer = new UtilityStates.Timer<MonoStateMachine>(machine,UnityEngine.Random.Range(1,20), "TimerEnd");
                NewTimer.PreviousState = this;
                NewTimer.TimerFinshed += () =>
                {
                    foreach(var Targets in machine.Cubes)
                    {
                        Targets.GetComponent<Renderer>().material.color = Color.green;
                    }
                    
                };
                machine.AddState(NewTimer);
            }

            Cube.GetComponent<Renderer>().material.color = Color.gray;
        }

        public override void Tick()
        {
            Count++;
            Cube.GetComponent<Renderer>().material.color = Color.blue;
            if (Count > 1000)
            {
                Count = 0;
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId, StateChange.Next,Name);
            }
        }

        public override void Reset()
        {
            base.Reset();
            Count = 0;
        }
    }

}

