using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceType : MonoBehaviour
{
    public enum resourceType { Stone, Wood, Metal}
    public resourceType currentResource = resourceType.Stone;

}
