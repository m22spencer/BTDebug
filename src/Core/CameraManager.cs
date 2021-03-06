using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using RuntimeInspectorNamespace;

using BattleTech;
using BattleTech.Rendering;

using BTDebug.Utils;
using BTDebug.BTCamera;

namespace BTDebug {
  public class CameraManager {
    private static CameraManager instance;
    private static string CombatGameCameraGOName = "GameCamera(Clone)";

    public bool IsFreeformCameraEnabled { get; private set; } = false;

    private GameObject GameCameraObject { get; set; }
    private CameraControl CameraControl { get; set; }
    private Camera Camera { get; set; }
    private FreeFormCamera FreeFormCamera { get; set; }

    private float originalCameraFoV = 0;
    private float originalCameraFarClipPlane = 0;

    public static CameraManager GetInstance() { 
      if (instance == null) instance = new CameraManager();
      return instance;
    }

    private CameraManager() {

    }

    public void ToggleFreeformCamera() {
      if (SetupCamera()) {
        EnableFreeformCamera(!IsFreeformCameraEnabled);
      }
    }

    public void EnableFreeformCamera(bool flag) {
      if (flag) {
        Main.Logger.LogDebug($"[BTDebug] Turning Freeform Camera is ON");
        originalCameraFoV = Camera.fieldOfView;
        originalCameraFarClipPlane = Camera.farClipPlane;
        Camera.fieldOfView = 60;
        Camera.farClipPlane = 9999;
        CameraControl.DEBUG_TakeCompleteControl = true;
        CameraControl.enabled = false;
        
        if (!FreeFormCamera) {
          FreeFormCamera = GameCameraObject.AddComponent<FreeFormCamera>();
        }

        FreeFormCamera.enabled = true;
        IsFreeformCameraEnabled = true;
      } else {
        Main.Logger.LogDebug($"[BTDebug] Turning Freeform Camera is OFF");
        CameraControl.enabled = true;
        CameraControl.DEBUG_TakeCompleteControl = false;
        Camera.fieldOfView = originalCameraFoV;
        Camera.farClipPlane = originalCameraFarClipPlane;
        if (FreeFormCamera) FreeFormCamera.enabled = false;
        IsFreeformCameraEnabled = false;
      }
    }

    private bool SetupCamera() {
      if (!GameCameraObject) {
        GameCameraObject = GameObject.Find(CombatGameCameraGOName);
        if (!GameCameraObject) return false;
        
        CameraControl = GameCameraObject.GetComponent<CameraControl>();
        if (!CameraControl) return false;

        Camera = GameCameraObject.GetComponentInChildren<Camera>();
        if (!Camera) return false;
      }
      return true;
    }
  }
}