using System;
using Cinemachine;
using UnityEngine;



public enum GameCameraMode
{
    Default,
    ADS,
    Unlocked
}

[System.Serializable]
public struct GameCamFov
{
    [Range(0.5f, 1)]
    public float UpdateSpeed;
    [Range(50, 120)]
    public float DefaultFOV;
    [Range(10, 50)]
    public float ADSfov;
}

public class GameCamera : MonoBehaviour
{
    public CinemachineVirtualCamera _VirtualCamComp;
    private GameCameraMode _cameraMode = GameCameraMode.Default;
    private Camera _CamComp;
    public Camera Camera => _CamComp;
    private Vector3 PositionOffset;
    private Vector3 LookAngle;

    public GameCamFov FOVsetup;
    public float XRotationLimit = 60;
    public float RotationSensitivity = 1.0f;

    public LayerMask VisionLayerMask;
    public Material PostProcessEffect;

    public PostShaderSetup ShaderSetup;
    // Start is called before the first frame update
    void Start()
    {
        PositionOffset = this.transform.localPosition;
        LookAngle = this.transform.rotation.eulerAngles;

        _CamComp = transform.root.gameObject.GetComponentInChildren<Camera>();
        _VirtualCamComp = transform.root.gameObject.GetComponentInChildren<CinemachineVirtualCamera>();
    }


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!PostProcessEffect) return;
        else
        {
            PostProcessEffect.SetInt("_Pixels", ShaderSetup.Pixels);
            PostProcessEffect.SetFloat("_OutlineLength", ShaderSetup.OutlineSize);
            Graphics.Blit(src, dest, PostProcessEffect);
        }
    }

    private void ClampRotation()
    {
        if (LookAngle.x > XRotationLimit)
        {
            LookAngle.x = XRotationLimit;
        }
        else if (LookAngle.x < -XRotationLimit)
        {
            LookAngle.x = -XRotationLimit;
        }
    }

    public Quaternion GetRotation()
    {
        return transform.rotation;
    }

    public Vector3 GetWorldAimPoint()
    {
        var forwardVector = transform.rotation * Vector3.forward;
        var AimPoint = this.transform.position + (forwardVector * 100.0f);

        Ray CameraRay = new Ray(transform.position, transform.rotation * Vector3.forward);

        RaycastHit hitInfo;
        var AimPointHit = Physics.Raycast(CameraRay, out hitInfo, 1000, VisionLayerMask);
        if (AimPointHit)
        {
            AimPoint = hitInfo.point;
        }

        return AimPoint;
    }

    public Vector3 Get2DForwardVector()
    {
        Quaternion flatQuat = Quaternion.Euler(0, LookAngle.y, 0);
        return flatQuat * Vector3.forward;
    }

    public void UpdateTransform()
    {
        transform.rotation = Quaternion.Euler(LookAngle);
        var worldPos = transform.parent.position + (Quaternion.Euler(LookAngle) * PositionOffset);
        this.transform.position = worldPos;
    }

    public void Rotate(Vector2 rotateVector)
    {
        LookAngle += new Vector3(
            -rotateVector.y,
            rotateVector.x,
            0
        ) * RotationSensitivity;
        ClampRotation();
    }

    public void SetMode(GameCameraMode newMode){
        _cameraMode = newMode;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _CamComp = transform.root.gameObject.GetComponentInChildren<Camera>();
        // Update V Cam Component
        if (_VirtualCamComp){
            _VirtualCamComp.Follow = this.transform;
        }
        UpdateTransform();
        ClampRotation();
    }

    void LateUpdate(){
        UpdateTransform();
        ClampRotation();
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, GetWorldAimPoint(), Color.green);
    }
}
