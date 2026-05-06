using UnityEngine;
using System;

namespace Game.Scripts.ZombieModules
{
    public class ZombieAnimationModule : ZombieBaseModule
    {
        [SerializeField] private Animator _animator;
        public override void Initialize(ZombieController zombieController)
        {
            base.Initialize(zombieController);
        }

        public void ActivateAttackAnim()
        {
            
        }

        public void ActivateRunningAnim()
        {
            
        }
    }
}