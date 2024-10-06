using Script.Core;
using Stateless;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Switcher
{
    public class Switch : MonoBehaviour
    {
        [Header("Machine")]
        private StateMachine<Command, Trigger> _stateMachine;
        
        [Header("Commands")]
        public Command idleCommand = new Command("Idle");
        public Command onCommand = new Command("On");
        public Command offCommand = new Command("Off");

        [Header("Triggers")]
        public Trigger onTrigger = new Trigger("On");
        public Trigger offTrigger = new Trigger("Off");

        [Header("UI Indicator")] 
        public TextMeshProUGUI currentAction;

        [Header("UI")] 
        public Button changeAction;
        
        private void Start()
        {
            _stateMachine = new StateMachine<Command, Trigger>(idleCommand);
            
            _stateMachine
                .Configure(onCommand)
                .OnEntry(OnChestOpened)
                .Permit(offTrigger, offCommand);

            _stateMachine
                .Configure(offCommand)
                .OnEntry(OnChestClosed)
                .Permit(onTrigger, onCommand);
                
            changeAction.onClick.AddListener(ChangeAction);

            UpdateUI();
        }

        private void OnChestClosed()
        {
            Debug.Log("Closed");
        }
        private void OnChestOpened()
        {
            Debug.Log("Opened");
        }
        
        private void ChangeAction()
        {
            UpdateUI();
        }
    
        private void UpdateUI()
        {
            currentAction.text = $"Current Action: {_stateMachine.State.id}";
        }
        
    }
}