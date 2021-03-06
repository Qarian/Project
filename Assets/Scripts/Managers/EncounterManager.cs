using System;
using System.Collections.Generic;
using Character;
using Difficulty;
using Encounter;
using Sirenix.OdinInspector;
using Timing;
using UI.Cards;
using UI.Entities;
using UI.Timeline;
using UnityEngine;
using Utilities;
using Zenject;
using Random = UnityEngine.Random;

namespace Managers
{
    public class EncounterManager : MonoBehaviour
    {
        [SerializeField] private PossibleEncounters possibleEncounters;

        [SerializeField] private List<Color> colorsForRepeatingEnemies = new();
        
        private UICardsController uiCards;
        private EntitiesLayoutManager layoutManager;
        private PlayerEntity player;

        private List<EnemyEntity> enemies;
        private CombatState state;

        private bool playerActionsEnabled;
        public bool PlayerActionsEnabled => playerActionsEnabled;
        public event Action<bool> OnPlayerActionsChange;

        [Inject]
        private void Init(UICardsController uiCards, EntitiesLayoutManager layoutManager, PlayerEntity playerEntity)
        {
            this.uiCards = uiCards;
            this.layoutManager = layoutManager;
            player = playerEntity;
        }

        
        private void Start()
        {
            PlayerGlobalData.Stats.NewEncounter();
            player.Initialize();
            Combination enemies = GenerateEnemies();
            DifficultyScaler.NewEncounter(enemies);
            uiCards.Init();

            EnablePlayerActions();
            TimeManager.Paused = true;
        }

        [Button]
        private Combination GenerateEnemies()
        {
            CombinationGroup combinationGroup =
                possibleEncounters.combinationGroup[Random.Range(0, possibleEncounters.combinationGroup.Count)];

            Combination combination = combinationGroup.combinationsPerDifficulties[PlayerGlobalData.selectedDifficulty];


            enemies = layoutManager.CreateEnemies(combination);
            
            // Assigning color to multiple same enemies
            Dictionary<EntityData, int> enemyDataCounter = new();

            foreach (EnemyEntity enemyEntity in enemies)
            {
                if (enemyDataCounter.ContainsKey(enemyEntity.entityData))
                    enemyDataCounter[enemyEntity.entityData]++;
                else
                    enemyDataCounter.Add(enemyEntity.entityData, 0);
                
                EntityTimeline.Instance.TrackEntity(enemyEntity, colorsForRepeatingEnemies[enemyDataCounter[enemyEntity.entityData]]);
            }

            return combination;
        }

        private void StartCombat()
        {
            foreach (EnemyEntity enemy in enemies)
            {
                enemy.StartCombat();
                enemy.OnEntityDeath += () => EnemyDead(enemy);
            }

            player.OnEntityDeath += GameOver;
        }

        private void GameOver()
        {
            TimeManager.Paused = true;
            GameManager.Instance.EndEncounter(false);
        }

        private void EnemyDead(EnemyEntity enemy)
        {
            enemies.Remove(enemy);
            if (enemies.Count == 0)
                EndCombat();
        }
        
        private void EndCombat()
        {
            state = CombatState.Finish;
            TimeManager.Paused = true;
            GameManager.Instance.EndEncounter(true);
        }
        
        public void ActionPerformed()
        {
            if (state == CombatState.Start)
            {
                state = CombatState.Combat;
                StartCombat();
                TimeManager.Paused = false;
            }

            playerActionsEnabled = false;
            OnPlayerActionsChange?.Invoke(false);
        }

        public void EnablePlayerActions()
        {
            playerActionsEnabled = true;
            OnPlayerActionsChange?.Invoke(true);
        }

        public void DrawCardToHand()
        {
            uiCards.DrawCardToHand();
        }

        private enum CombatState
        {
            Start, Combat, Finish
        }
    }
}
