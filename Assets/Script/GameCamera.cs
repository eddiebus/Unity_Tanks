using System;
using Cinemachine;
using Unity.VisualScripting;
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
    public float _CurrentFov;

    public LayerMask VisionLayerMask;
    public Material PostProcessEffect;

    public PostShaderSetup ShaderSetup;
    // Start is called before the first frame update
    void Start()
    {
        _CurrentFov = FOVsetup.DefaultFOV;

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


    private void UpdateFov()
    {
        var fovLerp = 10.0f  * Time.deltaTime;
        switch (_cameraMode)
        {
            case GameCameraMode.Default:
                _CurrentFov = Mathf.Lerp(_CurrentFov, FOVsetup.DefaultFOV, fovLerp);
                break;

            case GameCameraMode.ADS:
                _CurrentFov = Mathf.Lerp(_CurrentFov, FOVsetup.ADSfov, fovLerp);
                break;

            default:
                break;
        }

        if (_VirtualCamComp)
        {
            _VirtualCamComp.m_Lens.FieldOfView = _CurrentFov;
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
        if (!_CamComp){
            return transform.position;
        }
        var forwardVector = transform.rotation * Vector3.forward;
        var AimPoint = this.transform.position + (forwardVector * 100.0f);

        var camTransform = _CamComp.transform;
        Ray CameraRay = new Ray(camTransform.position, camTransform.rotation * Vector3.forward);

        RaycastHit hitInfo;
        var AimPointHit = Physics.Raycast(CameraRay, out hitInfo, 100, VisionLayerMask, QueryTriggerInteraction.Ignore);
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
        );
        ClampRotation();
    }

    public void SetMode(GameCameraMode newMode)
    {
        _cameraMode = newMode;
    }

    void Update()
    {
        UpdateFov();
        BillboardObject.UpdateDirection(_CamComp);
    }

    void FixedUpdate()
    {
        _CamComp = transform.root.gameObject.GetComponentInChildren<Camera>();
        // Update V Cam Component
        if (_VirtualCamComp)
        {
            _VirtualCamComp.Follow = this.transform;
        }
        UpdateTransform();
        ClampRotation();
    }

    void LateUpdate()
    {
        UpdateTransform();
        ClampRotation();
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, GetWorldAimPoint(), Color.green);
    }
}
