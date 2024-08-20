using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using PuzzleManagement;

public static class DialogueAssistant
{
    public static NPCDialogue currentNpc;

    [YarnCommand("start_puzzle")]
    public static void StartPuzzle() {
        if (currentNpc == null)
        {
            Debug.LogError("Tried to start a null puzzle!");
            return;
        }
        NPCDialogue dialogue = currentNpc.GetComponent<NPCDialogue>();
        dialogue.currentState = "Incomplete";
        SaveSystem.SaveGame();
        dialogue.GetComponent<PuzzleProc>().PuzzleInit();
    }


    [YarnCommand("set_state")]
    public static void SetState(string state) {
        if (currentNpc == null)
        {
            Debug.LogError("Tried to set the state of a null NPC!");
            return;
        }
        NPCDialogue dialogue = currentNpc.GetComponent<NPCDialogue>();
        dialogue.currentState = state;
    }

    public static T GetValueOr<T>(this VariableStorageBehaviour storage, string variableName, T defaultValue)
    {
        bool hasValue = storage.TryGetValue(variableName, out T value);
        return hasValue ? value : defaultValue;
    }
}
