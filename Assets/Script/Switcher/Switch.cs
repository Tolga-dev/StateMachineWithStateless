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
        StateMachine<Command, string> _stateMachine;
        
        [Header("Commands")]
        public Command idleCommand = new Command("Idle");
        public Command onCommand = new Command("On");
        public Command offCommand = new Command("Off");

        [Header("UI Indicator")] 
        public TextMeshProUGUI currentAction;

        [Header("UI")] 
        public Button changeAction;
        
        private void Start()
        {
            _stateMachine = new StateMachine<Command, string>(idleCommand);
            
            _stateMachine.Configure(onCommand).Permit("Sex", offCommand);
            _stateMachine.Configure(offCommand).Permit("Sex", onCommand);
                
            changeAction.onClick.AddListener(ChangeAction);

            UpdateUI();
        }

        private void ChangeAction()
        {
            _stateMachine.Fire("Sex");

            UpdateUI();
        }
    
        private void UpdateUI()
        {
            currentAction.text = $"Current Action: {_stateMachine.State.id}";
        }
        
    }
}