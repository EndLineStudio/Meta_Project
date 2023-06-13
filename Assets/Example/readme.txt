Photon Fusion KCC Example - Readme

Controls
================================================================

1) Keyboard + Mouse (PC, Editor)
----------------------------------------
Mouse            - Look
W,S,A,D          - Move
Shift            - Run
Space            - Jump
Tab              - Dash
+,-              - Toggle speed
Enter            - Lock/unlock cursor
Ctrl + Shift + M - Simulate app pause/resume
Q,E              - Side walk for testing smoothness
T + Numbers      - Teleport to specific areas (if available)
F4               - Toggle input smoothing
F5               - Toggle target frame rate
F6               - Toggle quality
F7               - Toggle vertical synchronization

2) Touch (Mobile)
----------------------------------------
Left virtual joystick  - Move
Right virtual joystick - Look
Double tap             - Jump
Sprint is triggered when Move joystick is far enough from touch origin.

3) Controllers (VR, Editor)
----------------------------------------
Left joystick  - Move
Right joystick - Look

4) Gamepad (All platforms)
----------------------------------------
Left joystick  - Move
Right joystick - Look
Left trigger   - Sprint
Right trigger  - Jump
A Button       - Dash

5) UI
----------------------------------------
Speed           - Toggle player speed (character base speed will be slower/faster)
Input Smoothing - Toggle input smoothing (look rotation will be smoother at the cost of increased input lag)
FPS             - Toggle target frame rate
Quality         - Toggle quality settings (URP settings, post-processing, lighting, ...)
V-Sync          - Toggle vertical synchronization


Testing
================================================================
The Village scene represents a common composition of block prefabs without optimizations. This is a default testing scene.
The Playground scene is used for testing specific cases (combinations of Box/Sphere/Mesh colliders, various angles, ...) and contains some high-poly models which won't run on low-end devices (mobile).


VR
================================================================
To enable VR, open Project Settings => XR Plug-in Management => Setup platform / providers => Enable 'Initialize XR on Startup'.
Sometimes XR Ray Interactor from XR Interaction Toolkit refuse to work on both Oculus device and Editor + Link. In case of problems:
1) Try to enable all VR related objects in the scene:
- Network Debug Start => VR => [ALL GAME OBJECTS]
- SceneConfig => SceneCamera => VR Camera
- SceneConfig => VR Event System
2) Remove Graphics Raycaster (NOT Tracked Device Graphics Raycaster) component from:
- Network Debug Start => VR => Canvas


Known bugs
================================================================
1) Slight movement while on MovingPlatform with horizontal movement.
2) Incorrect depenetration from MeshCollider with Convex option enabled. Nothing we can do about, this needs to be fixed in Unity/PhysX.
https://issuetracker.unity3d.com/issues/physics-dot-computepenetration-detects-a-collision-with-capsulecollider-if-meshcollider-convex-is-enabled-when-there-is-no-collision
As a workaround, don't use Convex toggle on MeshColliders or enable "Suppress Convex Mesh Colliders" in KCC settings.
