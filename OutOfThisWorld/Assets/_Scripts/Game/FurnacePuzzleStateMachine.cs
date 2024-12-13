using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    enum FurnacePuzzleState {
        Disabled,
        WaitStart,
        Ongoing,
        Won
    }

    public class FurnacePuzzleStateMachine : BooleanStateMachine {
        public float TotalCycleTime;
        public float WinThreshold;

        [Header("Triggers")]
        public List<Triggerable> OnFail;
        public List<Triggerable> OnWin;

        [Header("Furnace Indicator References")]
        public Renderer Indicator;
        public Material IndicatorDisabled;
        public Material IndicatorBad;
        public Material IndicatorGood;

        [Header("Furnace Computer References")]
        public Renderer Computer;
        public Material ComputerDisabled;
        public Material ComputerWaitStart;
        public Material ComputerOngoing;

        private FurnacePuzzleState _currentState;
        private float _beginAttemptTimestamp;

        void FixedUpdate() {
            if (_currentState == FurnacePuzzleState.Ongoing) {
                if (Time.fixedTime > _beginAttemptTimestamp + TotalCycleTime) {
                    Fail();
                } else if(Time.fixedTime > _beginAttemptTimestamp + TotalCycleTime*WinThreshold) {
                    Indicator.material = IndicatorGood;
                } 
            }
        }

        public override bool GetState() {
            return _currentState == FurnacePuzzleState.Won;
        }

        public void Activate() {
            if (_currentState == FurnacePuzzleState.Disabled) {
                _currentState = FurnacePuzzleState.WaitStart;
                Computer.material = ComputerWaitStart;
            }
        }

        public void Click() {
            if (_currentState == FurnacePuzzleState.WaitStart) {
                _currentState = FurnacePuzzleState.Ongoing;
                Computer.material = ComputerOngoing;
                Indicator.material = IndicatorBad;
                _beginAttemptTimestamp = Time.fixedTime;
            } else if (_currentState == FurnacePuzzleState.Ongoing) {
                if (Time.fixedTime > _beginAttemptTimestamp + TotalCycleTime*WinThreshold) {
                    Succeed();
                } else {
                    Fail();
                }
                
            }
        }

        void Fail() {
            _currentState = FurnacePuzzleState.WaitStart;
            Computer.material = ComputerWaitStart;
            Indicator.material = IndicatorDisabled;
            foreach(Triggerable trig in OnFail) {
                trig.Trigger();
            }
        }

        void Succeed() {
            _currentState = FurnacePuzzleState.Won;
            Computer.material = ComputerDisabled;
            Indicator.material = IndicatorDisabled;
            foreach(Triggerable trig in OnWin) {
                trig.Trigger();
            }
        }
    }
}
