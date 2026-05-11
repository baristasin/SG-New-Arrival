using UnityEngine;
using System;

namespace Game.Scripts.ZombieModules
{
    public enum ZombieAnimState { Idle, Walk, Run, Attack, Death, Dance }                                           

    public class ZombieAnimationModule : ZombieBaseModule
    {
        [SerializeField] private Animator _animator;
        
        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");
        private static readonly int DanceTrigger = Animator.StringToHash("Dance");
                                                                                                    
        private ZombieAnimState _currentState;

        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
            _currentState = ZombieAnimState.Idle;
            _animator.SetBool(IsRunning, false);
            _animator.ResetTrigger(AttackTrigger);
        }

        public void ActivateAttackAnim()
        {
            
        }

        public void ActivateRunningAnim()
        {
            
        }
        
        public void Play(ZombieAnimState state)
        {
            if (_currentState == state) return;
            _currentState = state;                                                                    
   
            switch (state)                                                                            
            {       
                case ZombieAnimState.Idle:
                    _animator.SetBool(IsRunning, false);
                    break;                                                                            
                case ZombieAnimState.Run:
                    _animator.SetBool(IsRunning, true);                                               
                    break;
                case ZombieAnimState.Attack:
                    _animator.SetBool(IsRunning, false);
                    _animator.SetTrigger(AttackTrigger);
                    break;
                case ZombieAnimState.Death:
                    _animator.SetBool(IsRunning, false);
                    _animator.SetTrigger(DeathTrigger);
                    break;
                case ZombieAnimState.Dance:
                    _animator.SetBool(IsRunning, false);
                    _animator.SetTrigger(DanceTrigger);
                    break;
            }                                                                                         
        }     
        
        public float GetCurrentClipLength()                                                           
        {
            return _animator.GetCurrentAnimatorStateInfo(0).length;                                   
        }  
    }
}