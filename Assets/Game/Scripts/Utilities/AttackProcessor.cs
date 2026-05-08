using System.Collections.Generic;
using Game.Scripts.BuildingModules;
using Game.Scripts.PlayerModules;
using Game.Scripts.ZombieModules;
using UnityEngine;

namespace Game.Scripts.Utilities
{
    public struct AttackRequest
    {
        public ZombieAttackTarget TargetType;
        public int Damage;
        public float ZombieX;
        public float ZombieZ;
    }

    public static class AttackProcessor
    {
        private static readonly Queue<AttackRequest> _queue = new(64);
        private static int _processPerFrame = 5;

        public static void Submit(ZombieAttackTarget targetType, int damage, float zombieX, float zombieZ)
        {
            _queue.Enqueue(new AttackRequest
            {
                TargetType = targetType,
                Damage = damage,
                ZombieX = zombieX,
                ZombieZ = zombieZ
            });
        }

        public static void ProcessQueue(PlayerHealthModule playerHealth,
            BuildingHealthModule buildingHealth, float dodgeThreshold)
        {
            int count = Mathf.Min(_processPerFrame, _queue.Count);

            for (int i = 0; i < count; i++)
            {
                var req = _queue.Dequeue();

                switch (req.TargetType)
                {
                    case ZombieAttackTarget.Building:
                        buildingHealth.TakeDamage(req.Damage);
                        break;

                    case ZombieAttackTarget.Player:
                        float dx = req.ZombieX - PlayerReference.X;
                        float dz = req.ZombieZ - PlayerReference.Z;
                        if (dx * dx + dz * dz < dodgeThreshold)
                            playerHealth.TakeDamage(req.Damage);
                        break;
                }
            }
        }
    }
}