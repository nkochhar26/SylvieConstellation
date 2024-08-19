using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public class StateMap
{
    [SerializeField] public List<Entry> states;

    public Entry Find(string name) => states.Find(e => e.name == name);
    public Entry Find(State state) => states.Find(e => e.state == state);

    [System.Serializable]
    public class Entry
    {
        [SerializeField] public string name;
        [SerializeField] public State state;
        [SerializeField] public string dialogueEntryTitle;
    }

    public enum State
    {
        FirstMeeting,
        Idle,
        IncompleteTask,
        CompletedTask,
        AllFinished,
    }
}
