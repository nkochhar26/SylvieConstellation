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
    public string currentState {
        get => dialogueRunner.VariableStorage.GetValueOr(stateVariable, "Error");
        set => dialogueRunner.VariableStorage.SetValue(stateVariable, value);
    }

    [SerializeField] private string defaultState;

    private bool canTalk;

    public DialogueRunner dialogueRunner;

    public GameObject gameUI;

    [SerializeField] public CharacterImageView characterImageView;
    [SerializeField] public Sprite charImage;
    private Sprite blankImage;

    // Start is called before the first frame update
    void Awake()
    {
        currentState = defaultState;
    }

    void Start()
    {
        blankImage = Resources.Load<Sprite>("blank");
    }

    // Update is called once per frame
    void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.playerIsInDialogue)
        {
            TalkToNpc();
        }
        if (!PlayerController.Instance.canMove && !dialogueRunner.IsDialogueRunning
            && GameManager.Instance.dialogueState == GameManager.DialogueState.NotTalking)
        {
            DoneTalking();
        }
    }

    void TalkToNpc()
    {
        gameUI.SetActive(false);
        PlayerController.Instance.canMove = false;
        GameManager.Instance.dialogueState = GameManager.DialogueState.Talking;

        dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = charImage;
        string scriptName = $"{dialogueId}{currentState}";
        DialogueAssistant.currentNpc = this;

        dialogueRunner.StartDialogue(scriptName);
    }

    void DoneTalking()
    {
        gameUI.SetActive(true);
        PlayerController.Instance.canMove = true;
        GameManager.Instance.dialogueState = GameManager.DialogueState.NotTalking;

        dialogueRunner.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = blankImage;
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
