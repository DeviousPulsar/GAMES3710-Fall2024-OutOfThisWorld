using UnityEngine;

namespace OutOfThisWorld {
    public class FurnacePuzzleClickTrigger : Triggerable {
        [SerializeField] FurnacePuzzleStateMachine Puzzle;

        public override void Trigger() {
            Puzzle.Click();
        }
    }
}
