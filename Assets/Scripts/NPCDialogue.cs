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
    [SerializeField] private string dialogueId;
    public string stateVariable => $"${dialogueId}State";

    /// <summary>
    /// The state of the NPC, which determines the dialogue upon interaction.
    /// </summary>
    public string currentState 
    {
        get => dialogueRunner.VariableStorage.GetValueOr(stateVariable, "Error");
        set => dialogueRunner.VariableStorage.SetValue(stateVariable, value);
    }

    public Sprite portrait
    {
        get => dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite;
        set => dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = value;
    }

    [SerializeField] private string defaultState;

    private bool canTalk;

    public DialogueRunner dialogueRunner;

    public GameObject gameUI;

    [SerializeField] public CharacterImageView characterImageView;
    [SerializeField] public Sprite charImage;
    public static Sprite blankImage;

    // Start is called before the first frame update
    void Awake()
    {
        currentState = defaultState;
    }

    void Start()
    {
        if (blankImage == null)
        {
            blankImage = Resources.Load<Sprite>("DialogueArt/Blank");
        }
        
        // Test if we're returning to the world...
        if (GameManager.Instance.lastInteractionId == dialogueId // After talking to this NPC...
            && GameManager.Instance.dialogueState == GameManager.DialogueState.Puzzle // And initiating a puzzle...
            && !GameManager.Instance.puzzleComplete) // But not completing it.
        {
            // In this scenario, let the player roam around or interact again.
            DoneTalking();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Test if we're returning to the world...
        if (GameManager.Instance.lastInteractionId == dialogueId // After talking to this NPC...
            && GameManager.Instance.dialogueState == GameManager.DialogueState.Puzzle // Initiating a puzzle...
            && GameManager.Instance.puzzleComplete) // And completing it.
        {
            // In this scenario, begin the dialogue for completing a puzzle.
            currentState = "Complete";
            TalkToNpc();
        }
        // Test if the player wants to begin an interaction (when the NPC is able to)
        else if (canTalk && Input.GetKeyDown(KeyCode.Space)
           && GameManager.Instance.dialogueState == GameManager.DialogueState.NotTalking)
        {
            TalkToNpc();
        }
        // Test if the player has finished an interaction, but we haven't acknowledged
        // that yet.
        else if (!PlayerController.Instance.canMove && !dialogueRunner.IsDialogueRunning
            && GameManager.Instance.dialogueState == GameManager.DialogueState.Talking)
        {
            DoneTalking();
        }
    }

    void TalkToNpc()
    {
        gameUI.SetActive(false);
        PlayerController.Instance.canMove = false;
        GameManager.Instance.dialogueState = GameManager.DialogueState.Talking;
        GameManager.Instance.lastInteractionId = dialogueId;

        portrait = charImage;
        DialogueAssistant.currentNpc = this;
        StartDialogue();
    }

    void StartDialogue()
    {
        string scriptName = $"{dialogueId}{currentState}";
        dialogueRunner.StartDialogue(scriptName);
    }

    void DoneTalking()
    {
        gameUI.SetActive(true);
        PlayerController.Instance.canMove = true;
        GameManager.Instance.dialogueState = GameManager.DialogueState.NotTalking;

        portrait = blankImage;
        DialogueAssistant.currentNpc = null;

        SaveSystem.SaveGame();
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
}
