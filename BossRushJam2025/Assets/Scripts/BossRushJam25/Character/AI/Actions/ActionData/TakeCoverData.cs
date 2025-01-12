using UnityEngine;

[CreateAssetMenu(fileName = "TakeCoverData", menuName = "Actions/TakeCoverData")]
public class TakeCoverData : AActionData
{
    [SerializeField] private float coverDetectionRadius = 10f;
    [SerializeField] private float distanceWithCover = 3f;

    public float CoverDetectionRadius => coverDetectionRadius;
    public float DistanceWithCover => distanceWithCover;
}
