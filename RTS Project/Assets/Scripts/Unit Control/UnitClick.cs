using UnityEngine;

public class UnitClick : MonoBehaviour
{
    private Camera myCamera;
    public GameObject Marker;

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask building;

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


            if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
            {
                print(hit.collider.gameObject);
                SelectUnits.Instance.BuildingSelected(hit.collider.gameObject);
            }

            if (Physics.Raycast(ray, out hit,Mathf.Infinity, clickable)) 
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    //wanneer shift ingedrukt wordt.
                    SelectUnits.Instance.ShiftClickSelect(hit.collider.gameObject);
                }
                else
                {
                    //wanneer 1 keer geklikt.
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

        //plaats marker voor AI om te volgen
        if (Input.GetMouseButtonDown(1)) 
        {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay (Input.mousePosition);

            if(Physics.Raycast(ray,out hit,Mathf.Infinity, ground))
            {
                Marker.transform.position = hit.point;
                Marker.SetActive(true);
            }
        }
    }
}
