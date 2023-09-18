using UnityEngine;

public class UnitClick : MonoBehaviour
{
    private Camera myCamera;

    public LayerMask clickable;
    public LayerMask ground;


    void Start()
    {
        myCamera = Camera.main;
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out hit,Mathf.Infinity, clickable)) 
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    //wanneer shift ingedrukt wordt.
                    Debug.Log("Multiple Units Selected");
                    SelectUnits.Instance.ShiftClickSelect(hit.collider.gameObject);
                }
                else
                {
                    //wanneer 1 keer geklikt.
                    Debug.Log("Unit Selected");
                    SelectUnits.Instance.ClickSelect(hit.collider.gameObject);
                }


            }
            else
            {
                //wanneer we missen en niet op shift klikken
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    SelectUnits.Instance.DeSelectAll();
                }


            }
        }
    }
}
