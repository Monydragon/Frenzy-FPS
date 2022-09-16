using System.Collections;
using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class ObjectiveKillEnemies : Objective
    {
        [Tooltip("Chose whether you need to kill every enemies or only a minimum amount")]
        public bool MustKillAllEnemies = true;
        [Tooltip("If true, The KillsToCompleteObjective will be set to the total amount of enemies")]
        public bool UseCurrentEnemyCount;
        [Tooltip("If MustKillAllEnemies is false, this is the amount of enemy kills required")]
        public int KillsToCompleteObjective = 5;

        [Tooltip("Start sending notification about remaining enemies when this amount of enemies is left")]
        public int NotificationEnemiesRemainingThreshold = 3;

        public float GetEnemiesCountDelay = 5f;

        private int m_KillTotal;


        private void Awake()
        {
        }

        protected override void Start()
        {
            base.Start();

            EventManager.AddListener<EnemyKillEvent>(OnEnemyKilled);
            EventManager.AddListener<EnemyAddedEvent>(OnEnemyAdded);
            EventManager.AddListener<EnemyRemovedEvent>(OnEnemyRemoved);

            // set a title and description specific for this type of objective, if it hasn't one
            if (string.IsNullOrEmpty(Title))
                Title = "Eliminate " + (MustKillAllEnemies ? "all the" : KillsToCompleteObjective.ToString()) +
                        " enemies";

            if (string.IsNullOrEmpty(Description))
                Description = GetUpdatedCounterAmount();
        }

        private void OnEnemyKilled(EnemyKillEvent evt)
        {
            if (IsCompleted)
                return;

            m_KillTotal++;

            if (MustKillAllEnemies)
                KillsToCompleteObjective = evt.RemainingEnemyCount + m_KillTotal;

            int targetRemaining = MustKillAllEnemies ? evt.RemainingEnemyCount : KillsToCompleteObjective - m_KillTotal;

            // update the objective text according to how many enemies remain to kill
            if (targetRemaining == 0)
            {
                CompleteObjective(string.Empty, GetUpdatedCounterAmount(), "Objective complete : " + Title);
            }
            else if (targetRemaining == 1)
            {
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? "One enemy left"
                    : string.Empty;
                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
            else
            {
                // create a notification text if needed, if it stays empty, the notification will not be created
                string notificationText = NotificationEnemiesRemainingThreshold >= targetRemaining
                    ? targetRemaining + " enemies to kill left"
                    : string.Empty;

                UpdateObjective(string.Empty, GetUpdatedCounterAmount(), notificationText);
            }
        }

        private string GetUpdatedCounterAmount()
        {
            return m_KillTotal + " / " + KillsToCompleteObjective;
        }

        private void OnEnemyAdded(EnemyAddedEvent evt)
        {
            if (UseCurrentEnemyCount)
            {
                KillsToCompleteObjective = evt.Amount;
            }
        }
        private void OnEnemyRemoved(EnemyRemovedEvent evt)
        {
            if (UseCurrentEnemyCount)
            {
                KillsToCompleteObjective = evt.Amount;
            }
        }
        private void OnDestroy()
        {
            EventManager.RemoveListener<EnemyKillEvent>(OnEnemyKilled);
            EventManager.RemoveListener<EnemyAddedEvent>(OnEnemyAdded);
            EventManager.RemoveListener<EnemyRemovedEvent>(OnEnemyRemoved);
        }
    }
}