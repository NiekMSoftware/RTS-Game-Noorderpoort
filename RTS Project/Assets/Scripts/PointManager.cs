using UnityEngine;

public class PointManager : MonoBehaviour
{
    public Points[] points;

    [System.Serializable]
    public enum EntityType
    {
        Player,
        AI
    }

    public enum PointType
    {
        none,
        resource,
        offensive,
        defensive,
    }

    [System.Serializable]
    public class ResourcePoint
    {
        public ItemData item;
        public float amount;
    }

    public void AddPoints(float amount, PointType pointType, EntityType type, ResourcePoint resourcePoint = null)
    {
        Points points = GetPointsByType(type);

        switch (pointType)
        {
            case PointType.resource:
                if (resourcePoint != null)
                {
                    resourcePoint.amount += amount;
                }

                points.totalResourceScore = 0;
                foreach (var oneOfTheResourcePoints in points.resourcePoints)
                {
                    points.totalResourceScore += oneOfTheResourcePoints.amount;
                }
                break;

            case PointType.offensive:
                points.offensiveScore += amount;
                break;

            case PointType.defensive:
                points.defensiveScore += amount;
                break;
        }
    }

    public Points GetPointsByType(EntityType type)
    {
        foreach (var point in points)
        {
            if (point.type == type)
            {
                return point;
            }
        }

        print("there is no points list with type : " + type);
        return null;
    }
}