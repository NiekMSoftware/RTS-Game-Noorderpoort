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
        defensive
    }

    public class Points
    {
        public Type type;
        public float resourcePoints;
        public float offensivePoints;
        public float defensivePoints;
    }

    public void AddPoints(float amount, PointType pointType, Type type)
    {
        Points points = GetPointsByType(type);

        switch (pointType)
        {
            case PointType.resource:
                points.resourcePoints += amount;
                break;

            case PointType.offensive:
                points.offensivePoints += amount;
                break;

            case PointType.defensive:
                points.defensivePoints += amount;
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