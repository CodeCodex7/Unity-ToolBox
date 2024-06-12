using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

//Oliver T, Generic State Engine based service to run State Machines

public enum StateChange { Next, Previous, Close }
public enum StateMachinceStates { Enabled, Disabled }

public class GenericStateEngine : MonoService<GenericStateEngine>
{
    #region Public Varibles
    public List<MonoStateMachine> LoadedMachines = new List<MonoStateMachine>();  // Could make a Dict because Uniqure ID
    #endregion

    #region Private Varibles
    public Action NextFrame;
    #endregion

    #region Unity Functions
    private void Awake()
    {
        RegisterService();
    }

    void Update()
    {
        if (NextFrame != null)
        {
            NextFrame.Invoke();
            NextFrame -= NextFrame;
        }
        
        foreach (MonoStateMachine m in LoadedMachines)
        {
            if (m.InteralState == StateMachinceStates.Enabled)
            {
                try
                {
                    foreach (State Target in m.ActiveStates.ToList())
                    {

                        Target.Tick();


                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }

    private void OnDestroy()
    {
        UnregisterService();
    }

    #endregion

    #region State Engine Functions
    /// <summary>
    /// Load a StateMachine
    /// </summary>
    public void LoadStateMachine()
    {
        foreach (MonoStateMachine m in LoadedMachines)
        {
            m.InteralState = StateMachinceStates.Enabled;
            foreach (State TargetState in m.ActiveStates)
            {
                TargetState.In();
            }

        }
    }
    public void LoadStateMachine(MonoStateMachine SM)
    {
        LoadedMachines.Add(SM);

        foreach (MonoStateMachine m in LoadedMachines)
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

    /// <summary>
    /// Unload a State Machine via it GUID id
    /// </summary>
    /// <param name="ID">ID of the StateMachine</param>
    public void UnloadStateMachine(Guid ID)
    {
        LoadedMachines.Remove(FindMachine(ID));
    }

    /// <summary>
    /// Change state of the finite state machine
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="ChangeMode"></param>
    public void ChangeState(Guid ID, StateChange ChangeMode, string Name)
    {
        MonoStateMachine Machine = FindMachine(ID);

        switch (ChangeMode)
        {
            case StateChange.Next:
                foreach (State Target in Machine.ActiveStates.ToList())
                {
                    if (Target.Name == Name)
                    {
                        Target.Out(); //Conduct out()  
                        Target.PreviousState = Machine.ActiveState; //Set Previouse State                        
                        Machine.ActiveStates.Remove(Target);

                        Machine.ActiveStates.Add(Target.NextStates.FirstOrDefault());
                        NextFrame += Machine.ActiveState.In;
                    }
                }

                break;
            //Still needs work may not work correctly
            case StateChange.Previous:
                foreach (State Target in Machine.ActiveStates.ToList())
                {
                    if (Target.Name == Name)
                    {
                        Target.Out();                      
                        Machine.ActiveStates.Remove(Target);
                        Machine.ActiveStates.Add(Target.PreviousState);
                        NextFrame += Machine.ActiveState.In;
                    }
                }
                break;
            case StateChange.Close:
                {
                    ExitState(ID, Name, false);
                }
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
    public void ChangeState(Guid ID, string StateName,string Name)
    {
        var Machine = FindMachine(ID);


        //Machine.ActiveState.Out();
        //Machine.ActiveState.PreviousState = Machine.ActiveState;
        //Machine.ActiveState = FindState(StateName);
        //NextFrame += Machine.ActiveState.In;
        var JumpState = FindState(StateName);

        foreach (State Target in Machine.ActiveStates.ToList())
        {
            if (Target.Name == Name)
            {
                Target.Out(); 
                Target.PreviousState = Machine.ActiveState;                      
                Machine.ActiveStates.Remove(Target);

                Machine.ActiveStates.Add(JumpState);
                NextFrame += JumpState.In;
            }
        }


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

    //TODO Ensure All states are added to statetable otherwise exit will crash

    /// <summary>
    /// Exit a state in a state machine
    /// </summary>
    /// <param name="ID">ID of State Machine</param>
    /// <param name="StateName">Name of the state to close</param>
    /// <param name="Immediately">Close state without ruinning it out componate </param>
    public void ExitState(Guid ID, string StateName, bool Immediately)
    {
        MonoStateMachine Machine = FindMachine(ID);

        State TargetState = (from Target in Machine.StateTable
                             where Target.Name == StateName
                             select Target).Single();

        if (!Immediately)
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
    public void ExitState(Guid ID, bool Immediately, params string[] StateName)
    {
        MonoStateMachine Machine = FindMachine(ID);

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
    public MonoStateMachine FindMachine(Guid ID)
    {
        var Machine = (from loadedMachine in LoadedMachines
                       where loadedMachine.MachineId == ID
                       select loadedMachine).Single();


        if (Machine != null)
        {
            return Machine;
        }
        else
        {
            Debug.Log(string.Format("Unable to find State Machine {0}", ID));
            return null;
        }

    }

    /// <summary>
    /// Find a State machine using it User set Name
    /// </summary>
    /// <param name="ID">ID of the State Machine, Each state machine has an Name </param>
    /// <returns></returns>
    public MonoStateMachine FindMachine(string ID)
    {
        var Machine = (from loadedMachine in LoadedMachines
                       where loadedMachine.Name == ID
                       select loadedMachine).Single();


        if (Machine != null)
        {
            return Machine;
        }
        else
        {
            Debug.Log(string.Format("Unable to find State Machine {0}", ID));
            return null;
        }

    }
    #endregion
}

#region StateMachine Template Classes
/// <summary>
/// Template State
/// </summary>
public abstract class State
{
    public State NextState
    {
        get { return m_NextStates[0]; }
        set { m_NextStates.Insert(0, value); }
    }
    public List<State> NextStates
    {
        get { return m_NextStates; }
        set { m_NextStates = value; }
    }
    public State PreviousState
    {
        get { return m_PreviouseState[0]; }
        set { m_PreviouseState.Insert(0, value); }
    }
    public List<State> PreviousStates
    {
        get { return m_PreviouseState; }
        //set { m_PreviouseState = value; }
    }

    private List<State> m_NextStates = new List<State>();
    //There may be a case where you want to have multiply previouse states but functionaily you only neeed one, this is refect in it property
    private List<State> m_PreviouseState = new List<State>();

    public string Name = "NotNamed";

    public abstract void In();

    public abstract void Tick();

    public abstract void Out();

    public virtual void Reset() { }

}

//Maybe Rethink having the statemachine as clases and insted impliment them via an inferface?
/// <summary>
/// Base State Machine with monobehaviour
/// </summary>
public abstract class MonoStateMachine : MonoBehaviour
{
    /// <summary>
    /// Identifier for the machine
    /// </summary>
    public Guid MachineId = Guid.NewGuid();

    [SerializeField]
    /// <summary>
    /// User Set Name of the State Machine for identfiction
    /// </summary>
    public string Name
    {
        get { return Name; }
        set { SetName(value); }//Should be using list function to modify list

    }

    /// <summary>
    /// User set Name for the server
    /// </summary>
    [SerializeReference] private string m_Name;
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

    /// <summary>
    /// The Interal State, Disabled = not running
    /// </summary>
    public StateMachinceStates InteralState = StateMachinceStates.Disabled;

    /// <summary>
    /// Search, StateTable by Name
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public bool SearchStateTable(string Name)
    {
        for (int i = 0; i < StateTable.Count; i++)
        {
            if (Name.Equals(StateTable[i].Name))
                return true;
        }
        return false;
    }
    /// <summary>
    /// Search, StateTable by type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool SearchStateTable(Type type)
    {
        for (int i = 0; i < StateTable.Count; i++)
        {
            if (StateTable[i].GetType().Equals(type))
                return true;
        }
        return false;
    }
    /// <summary>
    /// Add a state to the State Machine, adds to active state and StateTable
    /// </summary>
    /// <param name="state">State to add to state machine </param>
    public void AddState(State state)
    {
        M_ActiveStates.Add(state);
        StateTable.Add(state);
    }
    /// <summary>
    /// Add state to the State Table only.
    /// </summary>
    /// <param name="state">State to add to table</param>
    public void AddToStateTable(State state)
    {
        StateTable.Add(state);
    }
    private void SetName(string Name)
    {
        var GSM = Services.Resolve<GenericStateEngine>();
        
        foreach (var SM in GSM.LoadedMachines)
        {
            if(SM.Name.Equals(Name))
            {
                m_Name = string.Format("StateMachine_{0}", Time.realtimeSinceStartup);
                Debug.Log(string.Format("Name '{0}' Taken, Assigned name {1)",Name,m_Name));

                return;
            }
        }

        m_Name = Name;
    }
    public abstract void BuildStateTable();

}

///State Machine with a scriptableobject, anti pattern state machine need a singular base to exentd from for the GSE
/// <summary>
/// Base State Machine with SriptableObject
/// </summary>
public abstract class ScriptableStateMachine 
{
    /// <summary>
    /// Link to engine
    /// </summary>
    GenericStateEngine m_Engine;

    /// <summary>
    /// Identifier for the machine
    /// </summary>
    public Guid MachineId = Guid.NewGuid();

    [SerializeField]
    /// <summary>
    /// User Set Name of the State Machine for identfiction
    /// </summary>
    public string Name
    {
        get { return Name; }
        set { SetName(value); }//Should be using list function to modify list

    }

    /// <summary>
    /// User set Name for the server
    /// </summary>
    private string m_Name;
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

    /// <summary>
    /// The Interal State, Disabled = not running
    /// </summary>
    public StateMachinceStates InteralState = StateMachinceStates.Disabled;

    /// <summary>
    /// Search, StateTable by Name
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public bool SearchStateTable(string Name)
    {
        for (int i = 0; i < StateTable.Count; i++)
        {
            if (Name.Equals(StateTable[i].Name))
                return true;
        }
        return false;
    }
    /// <summary>
    /// Search, StateTable by type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool SearchStateTable(Type type)
    {
        for (int i = 0; i < StateTable.Count; i++)
        {
            if (StateTable[i].GetType().Equals(type))
                return true;
        }
        return false;
    }
    /// <summary>
    /// Add a state to the State Machine, adds to active state and StateTable
    /// </summary>
    /// <param name="state">State to add to state machine </param>
    public void AddState(State state)
    {
        M_ActiveStates.Add(state);
        StateTable.Add(state);
    }
    /// <summary>
    /// Add state to the State Table only.
    /// </summary>
    /// <param name="state">State to add to table</param>
    public void AddToStateTable(State state)
    {
        StateTable.Add(state);
    }
    private void SetName(string Name)
    {
        var GSM = Services.Resolve<GenericStateEngine>();

        foreach (var SM in GSM.LoadedMachines)
        {
            if (SM.Name.Equals(Name))
            {
                m_Name = string.Format("StateMachine_{0}", Time.realtimeSinceStartup);
                Debug.Log(string.Format("Name '{0}' Taken, Assigned name {1)", Name, m_Name));

                return;
            }
        }

        m_Name = Name;
    }
    public abstract void BuildStateTable();
}


#endregion