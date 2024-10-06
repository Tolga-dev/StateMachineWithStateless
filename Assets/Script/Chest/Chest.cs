using Script.Core;
using Stateless;
using UnityEngine;

namespace Script.Chest
{
    public class Chest : MonoBehaviour
    {
        public Command openChest = new Command("OpenChest");
        public Command closeChest = new Command("CloseChest");

        private StateMachine<Command, KeyCode> _stateMachine;

        private void Awake()
        {
            _stateMachine = new StateMachine<Command, KeyCode>(openChest);
            
            _stateMachine
                .Configure(openChest)
                .OnEntry(OnOpened)
                .Permit(KeyCode.Space, closeChest);

            _stateMachine
                .Configure(closeChest)
                .OnEntry(OnClosed)
                .Permit(KeyCode.Space, openChest);
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            ToggleState();
        }

        private void ToggleState() => _stateMachine.Fire(KeyCode.Space);

        private void OnClosed()
        {
            Debug.Log("On Closed");
        }

        private void OnOpened()
        {
            Debug.Log("On Opened");
        }
    }
}