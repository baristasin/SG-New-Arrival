using System;
using System.Collections.Generic;
using Game.Scripts.BehaviourTree;
using Game.Scripts.BehaviourTree.ZombieNodes;
using UnityEngine;

namespace Game.Scripts.ZombieModules
{
    public class ZombieController : MonoBehaviour
    {
        public Vector3 BuildingAttackingPosition => _buildingAttackingPosition;

        private Vector3 _buildingAttackingPosition;
        private int _tickGroup;
        private int _totalGroups;

        public ZombieActionModule ZombieActionModule => _zombieActionModule;
        public ZombieAnimationModule ZombieAnimationModule => _zombieAnimationModule;
        public ZombieCombatModule ZombieCombatModule => _zombieCombatModule;
        public ZombieHealthModule ZombieHealthModule => _zombieHealthModule;
        public ZombiePerceptionModule ZombiePerceptionModule => _zombiePerceptionModule;
        public ZombieMovementModule ZombieMovementModule => _zombieMovementModule;

        [SerializeField] private ZombieActionModule _zombieActionModule;
        [SerializeField] private ZombieAnimationModule _zombieAnimationModule;
        [SerializeField] private ZombieCombatModule _zombieCombatModule;
        [SerializeField] private ZombieHealthModule _zombieHealthModule;
        [SerializeField] private ZombiePerceptionModule _zombiePerceptionModule;
        [SerializeField] private ZombieMovementModule _zombieMovementModule;
        [SerializeField] private Collider _collider;

        private List<ZombieBaseNode> _allNodes = new List<ZombieBaseNode>();
        private Node _topNode;
        private bool _isInitialized;

        public void Setup(Vector3 buildingHitPoint, int tickGroup, int totalGroups)
        {
            _buildingAttackingPosition = buildingHitPoint;
            _tickGroup = tickGroup;
            _totalGroups = totalGroups;

            ZombieRegistry.Register(_collider, _zombieHealthModule);
            Initialize();
        }

        public void ZombieDead()
        {
            UnregisterZombie();
            // maybe animation first =>
            ZombieSpawnManager.DespawnZombie(this);
        }

        private void UnregisterZombie()
        {
            ZombieRegistry.Unregister(_collider);
        }

        private void Initialize()
        {
            _zombieActionModule.Initialize(this);
            _zombieAnimationModule.Initialize(this);
            _zombieCombatModule.Initialize(this);
            _zombieHealthModule.Initialize(this);
            _zombiePerceptionModule.Initialize(this);
            _zombieMovementModule.Initialize(this);

            _topNode = ConstructBehaviourTree();
            _isInitialized = true;
        }

        private Node ConstructBehaviourTree()
        {
            var checkHealthNode = new CheckHealthNode();
            var isBeingLuredNode = new IsBeingLuredNode();
            var waitAndProceedAttackNode = new WaitAndProceedAttackNode();
            var isInAttackRangeNode = new IsInAttackRangeNode();
            var tryToAttackNode = new TryToAttackNode();
            var goToAttackTargetNode = new GoToAttackTargetNode();

            _allNodes.Add(checkHealthNode);
            _allNodes.Add(isBeingLuredNode);
            _allNodes.Add(waitAndProceedAttackNode);
            _allNodes.Add(isInAttackRangeNode);
            _allNodes.Add(tryToAttackNode);
            _allNodes.Add(goToAttackTargetNode);

            foreach (var node in _allNodes)
            {
                node.Initialize(this);
            }


            var specialActionSelector = new Selector(new List<Node>
            {
                new Sequence(new List<Node>()), // HealOrBuff 
                new Sequence(new List<Node>()) // Berserk 
            });

            var surviveAndSpecialSequence = new Sequence(new List<Node>
            {
                checkHealthNode,
                specialActionSelector
            });

            Node CreateCombatBranch()
            {
                return new Selector(new List<Node>
                {
                    new Sequence(new List<Node>
                    {
                        isInAttackRangeNode,
                        waitAndProceedAttackNode,
                        tryToAttackNode
                    }),
                    new Sequence(new List<Node>
                    {
                        goToAttackTargetNode
                    })
                });
            }

            // 3. Attack Player Branch
            var attackPlayerSequence = new Sequence(new List<Node>
            {
                isBeingLuredNode,
                CreateCombatBranch()
            });

            // 4. Attack Building Branch
            var attackBuildingSequence = new Sequence(new List<Node>
            {
                CreateCombatBranch()
            });

            // ROOT SELECTOR
            return new Selector(new List<Node>
            {
                surviveAndSpecialSequence,
                attackPlayerSequence,
                attackBuildingSequence
            });
        }

        private void Update()
        {
            if (!_isInitialized) return;
            if (Time.frameCount % _totalGroups != _tickGroup) return;
            _topNode.Evaluate();
        }
    }
}