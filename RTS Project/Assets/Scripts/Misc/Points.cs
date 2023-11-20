[System.Serializable]
public class Points
{
    public PointManager.EntityType type;
    public float totalResourceScore;
    public PointManager.ResourcePoint[] resourcePoints;
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

    public PointManager.ResourcePoint GetResourcePointByItem(ItemData itemData)
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
