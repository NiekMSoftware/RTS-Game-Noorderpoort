using UnityEngine;

public class PointManager : MonoBehaviour
{
    public Points[] points;

    public enum Type
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

    [System.Serializable]
    public class Points
    {
        public Type type;
        public float totalResourceScore;
        public ResourcePoint[] resourcePoints;
        public float offensiveScore;
        public float defensiveScore;
        public float warScore
        {
            get
            {
                return offensiveScore + defensiveScore;
            }

            private set
            {
                warScore = value;
            }
        }

        public ResourcePoint GetResourcePointByItem(ItemData itemData)
        {
            foreach (var resourcePoint in resourcePoints)
            {
                if (resourcePoint.item == itemData)
                {
                    return resourcePoint;
                }
            }

            return null;
        }
    }

    public void AddPoints(float amount, PointType pointType, Type type, ResourcePoint resourcePoint = null)
    {
        Points points = GetPointsByType(type);

        switch (pointType)
        {
            case PointType.resource:
                points.totalResourceScore += amount;
                if (resourcePoint != null)
                {
                    resourcePoint.amount += amount;
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

    public Points GetPointsByType(Type type)
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