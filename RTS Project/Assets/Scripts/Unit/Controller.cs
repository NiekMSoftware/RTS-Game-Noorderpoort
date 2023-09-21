using System;
using UnityEngine;
public class Controller : MonoBehaviour {
    public float moveSpeed = 6f;
    
    Rigidbody _rigidbody;
    Camera _viewCamera;
    Vector3 _velocity;

    void Start() {
        this._rigidbody = this.GetComponent<Rigidbody>();
        this._viewCamera = Camera.main;
    }

    void Update() {
        Vector3 mousePos = this._viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                            Input.mousePosition.y, this._viewCamera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);

        this._velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized 
                         * this.moveSpeed;
    }

    void FixedUpdate() {
        this._rigidbody.MovePosition(this._rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}
