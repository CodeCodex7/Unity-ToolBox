using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

//Oliver.T 06-06-2024
public class TestMachine : StateMachine
{

    public List<GameObject> Cubes = new List<GameObject>();


    private void Start()
    {
        BuildStateTable();
        Services.Resolve<GenericStateEngine>().StartMachine(this);
    }


    /// <summary>
    /// Construct the interals of the state machine
    /// </summary>
    void BuildStateTable()
    {
        var BeginState = new Begin(this,"Begin");
        var MiddleState = new Middle(this,"Middle");
        var EndState = new End(this,"End");

        BeginState.NextStates.Add(MiddleState);
        MiddleState.NextStates.Add(EndState);
        EndState.NextStates.Add(BeginState);

        ActiveState = BeginState; //could also be ActiveState.Add(BeginState)

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
            Debug.Log(string.Format("My Type is {0}",GetType().ToString()));
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
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId,StateChange.Next);
                //Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId,StateChange.Jump,"End");
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
            Debug.Log(string.Format("My Type is {0}", GetType().ToString()));
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
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId, StateChange.Next);
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
            //machine.ActiveState = this;
            //Debug.Log(string.Format("Entering {0} in FSM {1}", MethodInfo.GetCurrentMethod(), machine.MachineId));
            Debug.Log(string.Format("My Type is {0}", GetType().ToString()));
        }

        public override void Out()
        {
            Cube.GetComponent<Renderer>().material.color = Color.gray;
            //Debug.Log(string.Format("Leaveing {0} in FSM {1}",MethodInfo.GetCurrentMethod(),machine.MachineId));
        }

        public override void Tick()
        {
            Count++;
            Cube.GetComponent<Renderer>().material.color = Color.blue;
            if (Count > 1000)
            {
                Count = 0;
                Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId, StateChange.Next);
                //Services.Resolve<GenericStateEngine>().ChangeState(machine.MachineId, StateChange.Jump, "Begin");
            }
        }
    }

}

