using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

//Oliver T, Generic State Engine based service to run State Machines
public enum StateChange {Next,Previouse,Close}
public enum StateMachinceStates { Enabled,Disabled}
public class GenericStateEngine : MonoService<GenericStateEngine>
{

    public List<StateMachine> LoadedMachines = new List<StateMachine>();
    public Action NextFrame;

    private void Awake()
    {
        RegisterService();
    }

    private void OnDestroy()
    {
        UnregisterService();
    }
    public void StartMachine()
    {
        foreach(StateMachine m in LoadedMachines)
        {
            m.InteralState = StateMachinceStates.Enabled;
           foreach(State TargetState in m.ActiveStates)
            {
                TargetState.In();
            }
            
        }
    }
    public void StartMachine(StateMachine SM)
    {
        LoadedMachines.Add(SM);

        foreach (StateMachine m in LoadedMachines)
        {
            if (m.InteralState == StateMachinceStates.Disabled)
            {
                m.InteralState = StateMachinceStates.Enabled;
                foreach (State Target in m.ActiveStates)
                {
                    Target.In();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NextFrame != null)
        {
            NextFrame.Invoke();
            NextFrame -= NextFrame;
        }

        foreach (StateMachine m in LoadedMachines)
        {
            if (m.InteralState == StateMachinceStates.Enabled)
            {
                foreach (State Target in m.ActiveStates.ToList())
                {
                    Target.Tick();
                }
            }
        }
    }

    /// <summary>
    /// Change state of the finite state machine
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="ChangeMode"></param>
    public void ChangeState(Guid ID,StateChange ChangeMode)
    {
        StateMachine Machine = FindMachine(ID);

        switch (ChangeMode)
        {
            case StateChange.Next:
                var StateList = Machine.ActiveStates;
                foreach (State Target in Machine.ActiveStates.ToList())
                {
                    Target.Out(); //Conduct out()
                 
                    Target.PreviousState = Machine.ActiveState; //Set Previouse State 
                    Machine.ActiveState = Target.NextStates.FirstOrDefault();
                    NextFrame += Machine.ActiveState.In;
                }

                break;
            case StateChange.Previouse:
                Machine.ActiveState.Out();
                Machine.ActiveState = Machine.ActiveState.PreviousState;
                NextFrame += Machine.ActiveState.In;
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Jump state of the finite state machine
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="ChangeMode"></param>
    /// <param name="StateName"></param>
    public void ChangeState(Guid ID,string StateName)
    {
        var Machine = (from loadedMachine in LoadedMachines
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();

        Machine.ActiveState.Out();
        Machine.ActiveState.PreviousState = Machine.ActiveState;
        Machine.ActiveState = FindState(StateName);
        NextFrame += Machine.ActiveState.In;



        State FindState(string stateName)
        {
            return (from A in Machine.StateTable
                    where A.Name == StateName
                    select A).Single();
        }


    }

    /* TODO - fiquare out multistate Changes
    public void ChangeState(Guid ID, params string[] States)
    {
        var Machine = (from loadedMachine in LoadedMachine
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();

        foreach

        Machine.ActiveState.Out();
        Machine.ActiveState.PreviouseState = Machine.ActiveState;
        Machine.ActiveState = FindState(StateName);
        NextFrame += Machine.ActiveState.In;



        State FindState(string stateName)
        {
            return (from A in Machine.StateTable
                    where A.Name == StateName
                    select A).Single();
        }


    }

    */

    /// <summary>
    /// Exit a state in a state machine
    /// </summary>
    /// <param name="ID">ID of State Machine</param>
    /// <param name="StateName">Name of the state to close</param>
    /// <param name="Immediately">Close state without ruinning it out componate </param>
    public void ExtiState(Guid ID,string StateName,bool Immediately)
    {
        StateMachine Machine = FindMachine(ID);

        State TargetState = (from Target in Machine.StateTable
                             where Target.Name == StateName
                             select Target).Single();

        if(!Immediately)
        {
            TargetState.Out();
        } 
        Machine.ActiveStates.Remove(TargetState); // Remove State from State Stack
    }

    /// <summary>
    /// Exit a state in a state machine
    /// </summary>
    /// <param name="ID">ID of State Machine</param>
    /// <param name="Immediately">Close state without ruinning it out componate </param>
    /// <param name="StateName">Name of the state to close</param>
    public void ExtiState(Guid ID, bool Immediately, params string[] StateName)
    {
        StateMachine Machine = FindMachine(ID);

        var TargetState = from Target in Machine.StateTable
                          where StateName.Contains(Target.Name)
                          select Target;

        foreach (var Target in TargetState)
        {
            if (!Immediately)
            {
                Target.Out();
            }
            Machine.ActiveStates.Remove(Target);  // Remove State from State Stack

        }
    }

    /// <summary>
    /// Find a State machine using it Guid(globally unique identifier)
    /// </summary>
    /// <param name="ID">ID of the State Machine, Each state machine has an ID </param>
    /// <returns></returns>
    public StateMachine FindMachine(Guid ID)
    {
        var Machine = (from loadedMachine in LoadedMachines
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();


        if(Machine != null)
        {
            return Machine;
        }
        else
        {
            return null;
        }

    }

}

#region StateMachineTemplateClasses
/// <summary>
/// Template State
/// </summary>
public abstract class State
{
    public List<State> NextStates = new List<State>();
    
    public State PreviousState
    {
        get { return m_PreviouseState[0]; }
        set { m_PreviouseState.Insert(0,value); }
    }

    public List<State> PreviousStates
    {
        get { return m_PreviouseState; }
        //set { m_PreviouseState = value; }
    }

    //There may be a case where you want to have multiply previouse states but functionaily you only neeed one, this is refect in it property
    public List<State> m_PreviouseState = new List<State>();

    public string Name;

    public abstract void In();

    public abstract void Tick();

    public abstract void Out();

}

//Maybe Rethink having the statemachine as clases and insted impliment them via an inferface?
/// <summary>
/// Base State Machine with monobehaviour
/// </summary>
public class StateMachine :MonoBehaviour
{

    /// <summary>
    /// Identifier for the machine
    /// </summary>
    public Guid MachineId = Guid.NewGuid();

    /// <summary>
    /// Get the Active State of the state machine
    /// </summary>
    public State ActiveState
    {
        get { return M_ActiveStates[0]; }
        set
        {
            if (M_ActiveStates.Count > 0)
            {
                M_ActiveStates[0] = value;
            }
            else
            {
                M_ActiveStates.Add(value);
            }

        }
    }

    /// <summary>
    /// Get the Active States of the StateMachine
    /// </summary>
    public List<State> ActiveStates
    {
        get { return M_ActiveStates; }
        //set { M_ActiveStates = value; }//Should be using list function to modify list
    }



    public List<State> M_ActiveStates = new List<State>();
    public List<State> StateTable = new List<State>();
    public StateMachinceStates InteralState = StateMachinceStates.Disabled;

}

/* State Machine with a scriptableobject, anti pattern state machine need a singular base to exentd from for the GSE
/// <summary>
/// Base State Machine with SriptableObject
/// </summary>
public class ScriptableStateMachine : ScriptableObject
{
    public Guid MachineId = Guid.NewGuid();
    public List<State> ActiveState;
    public List<State> StateTable = new List<State>();
    public StateMachinceStates InteralState = StateMachinceStates.Disabled;
}
*/

#endregion