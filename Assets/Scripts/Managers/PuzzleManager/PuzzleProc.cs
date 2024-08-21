using UnityEngine;
using UnityEngine.Events;

namespace PuzzleManagement {

    /// <summary>
    /// Sample component to Initiate a puzzle accounting for completion;
    /// </summary>
    public class PuzzleProc : MonoBehaviour {

        public static UnityEvent OnPuzzleComplete;

        [SerializeField] private PuzzleID puzzleID;

        private void Awake()
        {
            OnPuzzleComplete ??= new UnityEvent();
        }

        public void PuzzleInit() {
            bool complete = PuzzleManager.Instance.GetPuzzleStatus(puzzleID);
            if (complete) {
                OnPuzzleComplete?.Invoke();
                GameManager.Instance.puzzleComplete = true;
            } else {
                TransitionManager.Instance.GoToScene((int) puzzleID);
                GameManager.Instance.dialogueState = GameManager.DialogueState.Puzzle;
            }
        }
    }
}
