using BossRushJam25.Health;
using MoreMountains.Feedbacks;
using UnityEngine;

public class HealthVFX : MonoBehaviour
{
    [SerializeField] private MMF_Player lowHealthFeedbacksPlayer;
    [SerializeField] private MMF_Player hitFeedbacksPlayer;
    [SerializeField] private float lowHealthThreshold01;

    private HealthSystem health;

    public void Initialize(HealthSystem health)
    {
        this.health = health;
        health.OnHealthChanged.AddListener(HealthSystem_OnHealthChanged);
    }

    private void HealthSystem_OnHealthChanged(int current, int damageDelta)
    {
        TriggerHitFeedbacks(damageDelta);
        RefreshLowHealthFeedbacks();
    }

    private void TriggerHitFeedbacks(int damageDelta)
    {
        if(!hitFeedbacksPlayer.IsPlaying)
        {
            hitFeedbacksPlayer.PlayFeedbacks(position: Vector3.zero, feedbacksIntensity: Mathf.Max(1f, damageDelta));
        }
    }
    private void RefreshLowHealthFeedbacks()
    {
        if(health.Current == 0)
        {
            lowHealthFeedbacksPlayer.StopFeedbacks();
        }
        else if(health.Ratio < lowHealthThreshold01)
        {
            if(!lowHealthFeedbacksPlayer.IsPlaying)
            {
                lowHealthFeedbacksPlayer.PlayFeedbacks();
            }
        }
        else
        {
            if(lowHealthFeedbacksPlayer.IsPlaying)
            {
                lowHealthFeedbacksPlayer.StopFeedbacks();
            }
        }
    }
}
