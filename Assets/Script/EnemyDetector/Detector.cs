using Script.Core;
using Stateless;
using UnityEngine;

namespace Script.EnemyDetector
{
    public class Detector : MonoBehaviour
    {
        public Command idleCommand = new Command("Idle");
        public Command detectCommand = new Command("Detect");
        
        private StateMachine<Command, Command> _stateMachine;

        public Transform playerPos;
        public float maxDistance = 5;

        private bool IsPlayerClose() // not good implementation but for example it's ok
        {
            return Vector3.Distance(playerPos.position, transform.position) < maxDistance;
        }
        
        private void Awake()
        {
            _stateMachine = new StateMachine<Command, Command>(idleCommand);
    
            _stateMachine.Configure(idleCommand)
                .PermitIf(detectCommand, detectCommand, IsPlayerClose)  // Only allow transition if player is close
                .OnEntry(EnterIdleState)
                .OnExit(FoundPlayer);
    
            _stateMachine.Configure(detectCommand)
                .PermitIf(idleCommand, idleCommand, () => !IsPlayerClose())  // Only allow transition if player is far
                .OnEntry(EnterDetectState)
                .OnExit(PlayerIsNotOnDistance);
        }
  
        private void Update()
        {
            if (IsPlayerClose()) Detect();
            else Idle();
        }

        private void Idle()
        {
            if (!_stateMachine.IsInState(idleCommand))
                _stateMachine.Fire(idleCommand);
        }

        private void Detect()
        {
            if (!_stateMachine.IsInState(detectCommand))
                _stateMachine.Fire(detectCommand);
        }

        private void EnterIdleState()
        {
            Debug.Log("EnterIdleState State!");
        }

        private void EnterDetectState()
        {
            Debug.Log(" EnterDetectState State!");
        }
        
        private void PlayerIsNotOnDistance()
        {
            Debug.Log($"{_stateMachine.State.id} Player Is Not On Distance!");
        }

        private void FoundPlayer()
        {
            Debug.Log($"{_stateMachine.State.id} Player Is Found!");
        }
    }
}