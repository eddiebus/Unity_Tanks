using System;
using UnityEngine;

[Serializable]
public struct PostShaderSetup
{
    public int Pixels;
    public float OutlineSize;
    public float CameraDepth;
}

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
    private GameCameraMode _cameraMode = GameCameraMode.Default;
    public Camera _CamComp;
    private Vector3 PositionOffset;
    private Vector3 LookAngle;

    public GameCamFov FOVsetup;
    public float XRotationLimit = 60;
    public float RotationSensitivity = 1.0f;

    public LayerMask VisionLayerMask;
    public Material PostProcessEffect;
    public int ShaderPixels;
    public float ShaderOutline;

    public PostShaderSetup ShaderSetup;
    // Start is called before the first frame update
    void Start()
    {
        PositionOffset = this.transform.localPosition;
        LookAngle = this.transform.rotation.eulerAngles;
        _CamComp = GetComponent<Camera>();
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

    private void _SetFov()
    {
        switch (_cameraMode)
        {
            case GameCameraMode.Default:
                {
                    _CamComp.fieldOfView = Mathf.Lerp(
                        _CamComp.fieldOfView,
                        FOVsetup.DefaultFOV,
                        FOVsetup.UpdateSpeed
                        );
                    break;
                }
            case GameCameraMode.ADS:
                {
                    _CamComp.fieldOfView = Mathf.Lerp(
                        _CamComp.fieldOfView,
                        FOVsetup.ADSfov,
                        FOVsetup.UpdateSpeed
                        );
                    break;
                }
            default:
                _CamComp.fieldOfView = Mathf.Lerp(
                        _CamComp.fieldOfView,
                        FOVsetup.DefaultFOV,
                        FOVsetup.UpdateSpeed
                        );
                break;
        }
    }

    private void ClampRotation()
    {

        if (LookAngle.x > XRotationLimit * 0.5f)
        {
            LookAngle.x = XRotationLimit * 0.5f;
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
        var AimPointHit = Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out hitInfo, 1000, VisionLayerMask);
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
        Vector3 parentPos = this.transform.parent.position;
        var worldPos = transform.parent.position + (Quaternion.Euler(LookAngle) * PositionOffset);
        this.transform.position = worldPos;

        Ray testRay = new Ray();
        testRay.origin = transform.parent.position;
        testRay.direction = this.transform.rotation * PositionOffset;

        RaycastHit hitresult;
        bool hit = Physics.Raycast(testRay, out hitresult, PositionOffset.magnitude * 1.1f, VisionLayerMask);
        if (hit)
        {
            var hitVector = hitresult.point - transform.parent.position;

            transform.position = transform.parent.position + (hitVector * 0.7f);
        }
        Debug.Log($"Camera Arm Hit = {hit}");
    }

    public void Rotate(Vector2 rotateVector)
    {
        LookAngle += new Vector3(
            -rotateVector.y,
            rotateVector.x,
            0
        ) * RotationSensitivity;
    }

    public void SetMode(GameCameraMode newMode){
        _cameraMode = newMode;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
        ClampRotation();
        _SetFov();
    }

    void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, GetWorldAimPoint(), Color.green);
    }
}
