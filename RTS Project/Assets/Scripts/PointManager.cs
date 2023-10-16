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