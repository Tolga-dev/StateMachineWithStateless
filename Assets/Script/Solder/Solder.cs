using System.Collections;
using Script.Core;
using Stateless;
using UnityEngine;

namespace Script.Solder
{
    public class Soldier : MonoBehaviour
    {
        public Command idleCommand = new Command("Idle");
        public Command runCommand = new Command("Run");
        public Command walkCommand = new Command("Walk");
        public Command attackCommand = new Command("Attack");
        public Command cooldownCommand = new Command("Cooldown");
        public Command searchCommand = new Command("Search");

        private StateMachine<Command, Command> _stateMachine;

        public float detectionRange = 10f; // Distance within which soldier can detect the player
        public float attackRange = 2f;      // Distance within which soldier can attack the player
        public float walkSpeed = 2f;        // Speed when walking
        public float runSpeed = 5f;         // Speed when running
        
        public Transform player;             // Reference to the player
        
        public bool isOnCooldown = false;    // Whether the soldier is in cooldown
        public float cooldownTime = 1f;      // Duration of cooldown

        private void Start()
        {
            _stateMachine = new StateMachine<Command, Command>(idleCommand);
            ConfigureSoldierStates();
        }

        private void ConfigureSoldierStates()
        {
            // Idle State
            _stateMachine.Configure(idleCommand)
                .Permit(runCommand, runCommand)
                .Permit(walkCommand, walkCommand)
                .Permit(attackCommand, attackCommand);

            // Run State
            _stateMachine.Configure(runCommand)
                .Permit(idleCommand, idleCommand)
                .Permit(attackCommand, attackCommand)
                .Permit(cooldownCommand, cooldownCommand)
                .OnEntry(PerformRun);

            // Walk State
            _stateMachine.Configure(walkCommand)
                .Permit(runCommand, runCommand)
                .Permit(idleCommand, idleCommand)
                .Permit(attackCommand, attackCommand)
                .OnEntry(PerformWalk);

            // Attack State
            _stateMachine.Configure(attackCommand)
                .Permit(cooldownCommand, cooldownCommand)
                .Permit(idleCommand, idleCommand)
                .OnEntry(PerformAttack);

            // Cooldown State
            _stateMachine.Configure(cooldownCommand)
                .Permit(idleCommand, idleCommand)
                .Permit(searchCommand, searchCommand)
                .OnEntry(StartCooldown);

            // Search/Chase State
            _stateMachine.Configure(searchCommand)
                .Permit(runCommand, runCommand)
                .Permit(idleCommand, idleCommand);

            ExecuteTransition(idleCommand);
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= attackRange && !isOnCooldown)
            {
                ExecuteTransition(attackCommand);
            }
            else if (distanceToPlayer <= detectionRange && !isOnCooldown)
            {
                if (distanceToPlayer <= attackRange) 
                {
                    ExecuteTransition(attackCommand); // Attack if in range
                }
                else
                {
                    ExecuteTransition(runCommand); // Start running if within detection range
                }
            }
            else
            {
                ExecuteTransition(idleCommand); // Idle if too far
            }
        }

        private void ExecuteTransition(Command command)
        {
            if (_stateMachine.CanFire(command))
            {
                _stateMachine.Fire(command);
                Debug.Log($"Transitioned to: {command.id}");
            }
        } 

        // Commands
        private void PerformAttack()
        {
            Debug.Log("Attacking the player!");
            ExecuteTransition(cooldownCommand); 
        }

        private void PerformWalk()
        {
            Debug.Log("Walking towards the player!");
            StartCoroutine(WalkingRoutine());
        }

        private void PerformRun()
        {
            Debug.Log("Running towards the player!");
            StartCoroutine(RunningRoutine());
        }

        private IEnumerator WalkingRoutine()
        {
            while (_stateMachine.State == walkCommand)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                if (distanceToPlayer > detectionRange || isOnCooldown)
                {
                    ExecuteTransition(idleCommand);
                    yield break; // Exit coroutine if no longer walking
                }

                // Move towards the player
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * walkSpeed * Time.deltaTime;
                yield return null; // Wait until next frame
            }
        }

        private IEnumerator RunningRoutine()
        {
            while (_stateMachine.State == runCommand)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                if (distanceToPlayer > detectionRange || isOnCooldown)
                {
                    ExecuteTransition(idleCommand);
                    yield break; // Exit coroutine if no longer running
                }

                // Move towards the player at run speed
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * runSpeed * Time.deltaTime;
                yield return null; // Wait until next frame
            }
        }

        private void StartCooldown()
        {
            isOnCooldown = true;
            Debug.Log("Cooldown started");
            StartCoroutine(CooldownRoutine());
        }

        private IEnumerator CooldownRoutine()
        {
            yield return new WaitForSeconds(cooldownTime);
            isOnCooldown = false;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            ExecuteTransition(distanceToPlayer <= detectionRange ? runCommand : idleCommand);
        }
    }
}
