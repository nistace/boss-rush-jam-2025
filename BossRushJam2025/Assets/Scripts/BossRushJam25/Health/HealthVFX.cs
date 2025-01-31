using BossRushJam25.Health;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HealthVFX : MonoBehaviour
{
    [SerializeField] private MMF_Player feedbacksPlayer;
    [SerializeField] private float lowHealthThreshold01;

    private HealthSystem health;

    public void Initialize(HealthSystem health)
    {
        this.health = health;
        health.OnHealthChanged.AddListener(HealthSystem_OnHealthChanged);
    }

    private void HealthSystem_OnHealthChanged(int current, int damageDelta)
    {
        if(health.Current == 0)
        {
            feedbacksPlayer.StopFeedbacks();

            return;
        }

        if(health.Ratio < lowHealthThreshold01)
        {
            if(!feedbacksPlayer.IsPlaying)
            {
                feedbacksPlayer.PlayFeedbacks();
            }
        }
        else
        {
            if(feedbacksPlayer.IsPlaying)
            {
                feedbacksPlayer.StopFeedbacks();
            }
        }
    }
}
