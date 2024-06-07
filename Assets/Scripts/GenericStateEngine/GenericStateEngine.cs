using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public enum StateChange {Next,Previouse,Jump}
public enum StateMachinceStates { Enabled,Disabled}
public class GenericStateEngine : MonoService<GenericStateEngine>
{

    public List<StateMachine> LoadedMachine;
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
        foreach(StateMachine m in LoadedMachine)
        {
            m.InteralState = StateMachinceStates.Enabled;
            m.ActiveState.In();
        }
    }
    public void StartMachine(StateMachine SM)
    {
        LoadedMachine.Add(SM);

        foreach (StateMachine m in LoadedMachine)
        {
            if (m.InteralState == StateMachinceStates.Disabled)
            {
                m.InteralState = StateMachinceStates.Enabled;
                m.ActiveState.In();
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


        foreach (StateMachine m in LoadedMachine)
        {
            if (m.InteralState == StateMachinceStates.Enabled)
            {
                m.ActiveState.Tick();
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
        var Machine = (from loadedMachine in LoadedMachine
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();

        switch (ChangeMode)
        {
            case StateChange.Next:
                Machine.ActiveState.Out();
                Machine.ActiveState.PreviouseState = Machine.ActiveState;
                Machine.ActiveState = Machine.ActiveState.NextStates.FirstOrDefault();
                NextFrame += Machine.ActiveState.In;

                break;
            case StateChange.Previouse:
                Machine.ActiveState.Out();
                Machine.ActiveState = Machine.ActiveState.PreviouseState;
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
    public void ChangeState(Guid ID, StateChange ChangeMode,string StateName)
    {
        var Machine = (from loadedMachine in LoadedMachine
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();

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

}


public abstract class State
{
    public List<State> NextStates = new List<State>();
    public State PreviouseState = null;
    public string Name;

    public abstract void In();

    public abstract void Tick();

    public abstract void Out();

}


public class StateMachine :MonoBehaviour
{
    public Guid MachineId = Guid.NewGuid();
    public State ActiveState;
    public List<State> StateTable = new List<State>();
    public StateMachinceStates InteralState = StateMachinceStates.Disabled;

}
