using UnityEngine;

namespace OutOfThisWorld {
    public class FurnacePuzzleActivateTrigger : Triggerable {
        [SerializeField] FurnacePuzzleStateMachine Puzzle;

        public override void Trigger() {
            Puzzle.Activate();
        }
    }
}
