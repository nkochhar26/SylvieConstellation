using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Yarn.Unity;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


public enum PlayerConstellationState {
    PERSEUS,
    LOVERS,
    TRICKSTER,
    DIONYSUS,
    DRACO,
    CASSIOPEIA,
    GUN,
    MINOR1,
    MINOR2
}

public class NPCDialogue : MonoBehaviour
{
    private StateMap.State currentState;
    public StateMap.State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private bool canTalk;

    public DialogueRunner dialogueRunner;

    public GameObject gameUI;

    // Name of status variable to get from Dialog scripts
    [Header("Dialogue Script Status Variable")]
    [SerializeField] public string statusVar;

    [Header("Dialogue State Map")]
    [SerializeField] private StateMap stateMap;

    [SerializeField] public CharacterImageView characterImageView;
    [SerializeField] public Sprite charImage;
    private Sprite blankImage;

    // Start is called before the first frame update
    void Start()
    {
        blankImage = Resources.Load<Sprite>("blank");
        currentState = StateMap.State.Idle;
        EnterState(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.isInDialogueState)
        {
            //dialogueRunner.StartDialogue("LoversNPC");
            gameUI.SetActive(false);
            PlayerController.Instance.canMove = false;
            GameManager.Instance.isInDialogueState = true;

            dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = charImage;

            dialogueRunner.VariableStorage.TryGetValue($"${statusVar}", out string dialogueAnswer);
            ChangeDialogueState(stateMap.Find(dialogueAnswer).state);
            ExecuteState(currentState);
        }
        if (!PlayerController.Instance.canMove) {
            if (!dialogueRunner.IsDialogueRunning) {
                PlayerController.Instance.canMove = true;
                GameManager.Instance.isInDialogueState = false;
                dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = blankImage;
                gameUI.SetActive(true);
            }
        }
    }

    public void ChangeDialogueState(StateMap.State newState)
    {
        ExitState(currentState);
        currentState = newState;
        EnterState(newState);
        Debug.Log($"STATE: {currentState.GetType()}");
    }

    public void ExecuteState(StateMap.State state)
    {
        Debug.Log($"{name} executing state {state}");
        string dialogueScriptTitle = stateMap.Find(state).dialogueEntryTitle;

        switch (state)
        {
            case StateMap.State.Idle:
                dialogueRunner.StartDialogue(dialogueScriptTitle);
                ChangeDialogueState(StateMap.State.FirstMeeting);
                break;

            case StateMap.State.FirstMeeting:
                string dialogueAnswer;
                dialogueRunner.VariableStorage.TryGetValue($"${statusVar}", out dialogueAnswer);
                if (dialogueAnswer.Equals("Affirmative"))
                {
                    ChangeDialogueState(StateMap.State.IncompleteTask);
                }
                else
                {
                    ChangeDialogueState(StateMap.State.Idle);
                }
                break;

            case StateMap.State.AllFinished:
            case StateMap.State.CompletedTask:
            case StateMap.State.IncompleteTask:
                dialogueRunner.StartDialogue(dialogueScriptTitle);
                break;

            default:
                Debug.Log($"No action assigned to {state} state");
                break;
        }
    }

    public void EnterState(StateMap.State state)
    {
        Debug.Log($"{name} entering state {state}");

        switch (state)
        {
            case StateMap.State.CompletedTask:
                dialogueRunner.VariableStorage.SetValue($"${statusVar}", "TalkToNPCAgain");
                break;

            case StateMap.State.AllFinished:
            case StateMap.State.FirstMeeting:
            case StateMap.State.Idle:
            case StateMap.State.IncompleteTask:
                break;

            default:
                Debug.Log($"No action assigned to {state} state");
                break;
        }
    }

    public void ExitState(StateMap.State state)
    {
        Debug.Log($"{name} exiting state {state}");

        switch (state)
        {
            case StateMap.State.FirstMeeting:
            case StateMap.State.IncompleteTask:
                dialogueRunner.Stop();
                break;

            case StateMap.State.AllFinished:
            case StateMap.State.CompletedTask:
            case StateMap.State.Idle:
                break;

            default:
                Debug.Log($"No action assigned to {state} state");
                break;
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            canTalk = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) {
            canTalk = false;   
        }
    }

    [YarnCommand("show_image")]
    public void ShowImage(string filepath)
    {
        if (!filepath.Equals("NO SPRITE"))
        {

            //characterImageView.characterDialogueImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filepath);
            Debug.Log($"SPRITE: {characterImageView.characterDialogueImage.sprite}");

        }
        else
        {
            characterImageView.characterDialogueImage.sprite = null;
            Debug.Log($"SPRITE: {characterImageView.characterDialogueImage.sprite}");
        }
    }
}
