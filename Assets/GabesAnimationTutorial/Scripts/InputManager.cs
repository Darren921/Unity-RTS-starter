using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public static class InputManager
{

    private static readonly RTS_Inputs Inputs = new();
    private static Commander _commander;
    private static Camera _camera;

    private static float _cameraLerpPercent = 0.5f;
    private static readonly Vector3 CamPointA = new (0,20,-15);
    private static readonly Vector3 CamPointB = new (0,2,-2);

    public static void SetCommander(Commander newTarget)
    {
        _commander = newTarget;
        if(_camera) _camera.transform.SetParent(_commander.transform, true);
    }

    public static void SetControlledCamera(Camera newCam)
    {
        _camera = newCam;
        _camera.transform.SetParent(_commander.transform, true);
        HandleCameraLerp(0);
    }

    public static void Initialize(Commander commander, Camera cam = null)
    {

        //If the camera exists, use it. ELSE use the main camera.
        SetCommander(commander);
        SetControlledCamera(cam?cam:Camera.main);

        
        /*NOTE FOR BEGINNERS:
         
        You need {} when making anonymous functions longer than one line:
        Inputs.InGame.Settings.performed += _ => 
        {
            Debug.Log("Hello");
            SetUIMode();
        };
        
        Also, _ is called "Discard" and CTX means context. They are just common variable names... 
        Name them Rebecca, I don't care.
        */
        Inputs.InGame.Settings.performed += _ => SetUIMode();
        
        //Set commander controlled inputs
        Inputs.InGame.Attack.performed += _ => _commander.Attack(CamToWorldRay());
        Inputs.InGame.Attack.performed += _ => _commander.Marked(CamToWorldRay());
        Inputs.InGame.MoveTo.performed += _ => _commander.MoveTo(CamToWorldRay());

        //Set camera controlled inputs
        Inputs.InGame.CameraMovement.performed += ctx => _commander.SetMoveDirection(ctx.ReadValue<Vector2>());
        Inputs.InGame.CameraMovement.canceled += _ => _commander.SetMoveDirection(Vector2.zero);
        Inputs.InGame.CameraRotate.performed += ctx =>_commander.SetRotationDirection(ctx.ReadValue<float>());
        Inputs.InGame.CameraZoom.performed += ctx => HandleCameraLerp(ctx.ReadValue<float>());

        //Set UI inputs
        Inputs.UI.ExitUI.performed += _ => SetGameMode();
        //We want to bind ourselves to the camera, other things must then listen to us.   
        
        //Enable game mode settings
        SetGameMode();
    }

    private static void HandleCameraLerp(float val)
    {
        _cameraLerpPercent =
            Mathf.Clamp(_cameraLerpPercent + Time.deltaTime * Settings.Instance.MouseZoomSens * val,
                0, 1);
        _camera.transform.localPosition = Vector3.Slerp(CamPointA, CamPointB, _cameraLerpPercent);
    }

    private static Ray CamToWorldRay()
    {
        return _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    private static void SetUIMode()
    {
        Inputs.InGame.Disable();
        Inputs.UI.Enable();
    }

    private static void SetGameMode()
    {
        Inputs.InGame.Enable();
        Inputs.UI.Disable();
    }
}
