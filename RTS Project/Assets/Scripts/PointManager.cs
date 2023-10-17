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
    public class Points
    {
        public Type type;
        public float resourceScore;
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
    }

    public void AddPoints(float amount, PointType pointType, Type type)
    {
        Points points = GetPointsByType(type);

        switch (pointType)
        {
            case PointType.resource:
                points.resourceScore += amount;
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