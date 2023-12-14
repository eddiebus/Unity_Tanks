using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class GameCamera : MonoBehaviour
{
    private Camera _CamComp;
    public Vector3 PositionOffset;
    private Vector3 LookAngle;
    public float XRotationLimit = 80;
    public float RotationSensitivity = 1.0f;


    public LayerMask AimPointCollision;

    // Start is called before the first frame update
    void Start()
    {
        PositionOffset = this.transform.localPosition;
        LookAngle = this.transform.rotation.eulerAngles;
        _CamComp = GetComponent<Camera>();
    }

    private void ClampRotation(){

        if (LookAngle.x >   XRotationLimit * 0.5f){
            LookAngle.x = XRotationLimit * 0.5f;
        }
        else if (LookAngle.x < -XRotationLimit ){
            LookAngle.x =  -XRotationLimit;
        }
    }

    public Quaternion GetRotation(){
        return transform.rotation;
    }

    public Vector3 GetWorldAimPoint(){
        var forwardVector = transform.rotation * Vector3.forward;
        var AimPoint = this.transform.position + (forwardVector * 100.0f);

        Ray CameraRay = new Ray(transform.position,transform.rotation * Vector3.forward);
        
        RaycastHit hitInfo;
        var AimPointHit = Physics.Raycast(transform.position,transform.rotation * Vector3.forward,out hitInfo,1000,AimPointCollision);
        if (AimPointHit){
            AimPoint = hitInfo.point;
        }

        
        return AimPoint;
    }
    
    public Vector3 Get2DForwardVector(){
        Quaternion flatQuat = Quaternion.Euler(0,LookAngle.y,0);
        return flatQuat * Vector3.forward;
    }

    public void UpdateTransform(){
        transform.rotation = Quaternion.Euler(LookAngle);
        Vector3 parentPos = this.transform.parent.position;
        this.transform.position = transform.parent.position + (Quaternion.Euler(LookAngle) * PositionOffset);
    }

    public void Rotate(Vector2 rotateVector){
        LookAngle += new Vector3(
            -rotateVector.y,
            rotateVector.x,
            0
        ) * RotationSensitivity;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
        ClampRotation();
    }

    void OnDrawGizmos(){
        Debug.DrawLine(transform.position,GetWorldAimPoint(),Color.green);
    }
}
