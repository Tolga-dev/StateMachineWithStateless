using System;
using Script.Core;
using Stateless;
using UnityEngine;

namespace Script.Chest
{
    public class Chest : MonoBehaviour
    {
        public Command openChest = new Command("OpenChest");
        public Command closeChest = new Command("CloseChest");

        public Trigger openChestTrigger = new Trigger("OpenChestTrigger");
        public Trigger closeChestTrigger = new Trigger("CloseChestTrigger");
        
        private StateMachine<Command, Trigger> _stateMachine;

        private void Awake()
        {
            _stateMachine = new StateMachine<Command, Trigger>(closeChest);
            
            _stateMachine.Configure(openChest).OnEntry(OnOpened).Permit(openChestTrigger, closeChest);
            _stateMachine.Configure(closeChest).OnEntry(OnClosed).Permit(closeChestTrigger, openChest);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) Open();
            else if (Input.GetMouseButtonDown(1)) Close();
            
        }

        private void Open()
        {
            if (!_stateMachine.CanFire(openChestTrigger)) return;
            _stateMachine.Fire(openChestTrigger);
            
        }

        private void Close()
        {
            if (!_stateMachine.CanFire(closeChestTrigger)) return;
            _stateMachine.Fire(closeChestTrigger);
        }
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