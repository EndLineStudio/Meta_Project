namespace Example
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Helper script for UI and keyboard shortcuts to quickly navigate across level sections
	/// </summary>
	public sealed class Menu : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private GUISkin          _skin;
		[SerializeField]
		private bool             _showMenuUI;
		[SerializeField]
		private Section[]        _sections;
		[SerializeField]
		private SceneConfig      _sceneConfig;
		[SerializeField]
		private FrameRateUpdater _frameRateUpdater;

		private Player   _localPlayer;
		private Section  _currentSection;
		private bool     _showTeleports;
		private bool     _selectingLocation;
		private GUIStyle _defaultStyle;
		private GUIStyle _selectedStyle;

		// MonoBehaviour INTERFACE

		private void Update()
		{
			if (Keyboard.current == null)
				return;

			if (Keyboard.current.tKey.wasPressedThisFrame == true && _sections.Length > 0)
			{
				_showTeleports     = !_showTeleports;
				_currentSection    = default;
				_selectingLocation = default;
			}

			if (_showTeleports == true)
			{
				for (int i = 0, count = _sections.Length; i < count; ++i)
				{
					if (_selectingLocation == false && GetNumberDown(i) == true)
					{
						_currentSection    = _sections[i];
						_selectingLocation = true;

						return;
					}
				}

				if (_currentSection != null)
				{
					for (int i = 0, count = _currentSection.Locations.Length; i < count; ++i)
					{
						if (_selectingLocation == true && GetNumberDown(i) == true)
						{
							TeleportPlayer(_currentSection.Locations[i]);

							_showTeleports     = default;
							_currentSection    = default;
							_selectingLocation = default;

							return;
						}
					}
				}
			}

			if (Keyboard.current.f4Key.wasPressedThisFrame == true)
			{
				ToggleInputSmoothing();
			}

			if (Keyboard.current.f5Key.wasPressedThisFrame == true)
			{
				ToggleFrameRate();
			}

			if (Keyboard.current.f6Key.wasPressedThisFrame == true)
			{
				ToggleQualityLevel();
			}

			if (Keyboard.current.f7Key.wasPressedThisFrame == true)
			{
				ToggleVSync();
			}

			if (Keyboard.current.f8Key.wasPressedThisFrame == true && Application.isMobilePlatform == false && Application.isEditor == false)
			{
				ToggleFullScreen();
			}
		}

		private void OnGUI()
		{
			if (_showMenuUI == false)
				return;

			Initialize();

			if (Runner == null || Runner.IsRunning == false)
				return;

			float verticalSpace   = 5.0f;
			float horizontalSpace = 5.0f;

			GUILayout.BeginVertical();
			GUILayout.Space(verticalSpace);
			GUILayout.BeginHorizontal();
			GUILayout.Space(horizontalSpace);

			if (_sections.Length > 0)
			{
				if (GUILayout.Button("[T] Teleport", _showTeleports == true ? _selectedStyle : _defaultStyle) == true)
				{
					_showTeleports     = !_showTeleports;
					_currentSection    = default;
					_selectingLocation = default;
				}
			}

			float playerSpeed    = GetPlayerSpeed();
			float inputSmoothing = GetInputSmoothing();

			if (GUILayout.Button($"[+/-] Speed ({playerSpeed:F2}x)", _defaultStyle) == true)
			{
				TogglePlayerSpeed();
			}

			if (GUILayout.Button($"[F4] Input Smoothing ({(int)(inputSmoothing * 1000.0f + 0.1f)}ms)", _defaultStyle) == true)
			{
				ToggleInputSmoothing();
			}

			if (GUILayout.Button($"[F5] FPS ({(Application.targetFrameRate == 0 ? "Unlimited" : Application.targetFrameRate.ToString())} / {_frameRateUpdater.SmoothFrameRate})", _defaultStyle) == true)
			{
				ToggleFrameRate();
			}

			string qualityName  = "Default";
			int    qualityLevel = _sceneConfig.GetQualityLevel();

			switch (qualityLevel)
			{
				case 0: { qualityName = "Low";    break; }
				case 1: { qualityName = "Medium"; break; }
				case 2: { qualityName = "High";   break; }
			}

			if (GUILayout.Button($"[F6] Quality: {qualityName}", _defaultStyle) == true)
			{
				ToggleQualityLevel();
			}

			if (GUILayout.Button($"[F7] V-Sync ({(QualitySettings.vSyncCount == 0 ? "Off" : "On")})", _defaultStyle) == true)
			{
				ToggleVSync();
			}

			if (Application.isMobilePlatform == false && Application.isEditor == false)
			{
				if (GUILayout.Button($"[F8] FullScreen ({Screen.fullScreenMode})", _defaultStyle) == true)
				{
					ToggleFullScreen();
				}
			}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space(horizontalSpace);

			if (_showTeleports == true)
			{
				GUILayout.BeginVertical();

				for (int i = 0, count = _sections.Length; i < count; ++i)
				{
					string buttonText = _selectingLocation == false ? $"[{i + 1}] {_sections[i].Root.name}" : _sections[i].Root.name;
					if (GUILayout.Button(buttonText, _currentSection == _sections[i] ? _selectedStyle : _defaultStyle) == true)
					{
						if (_currentSection != _sections[i])
						{
							_currentSection    = _sections[i];
							_selectingLocation = true;
						}
						else
						{
							_currentSection    = default;
							_selectingLocation = default;
						}
					}
				}

				GUILayout.EndVertical();

				if (_currentSection != null)
				{
					GUILayout.BeginVertical();

					for (int i = 0, count = _currentSection.Locations.Length; i < count; ++i)
					{
						if (_currentSection.Locations[i] == null)
							continue;

						string buttonText = _selectingLocation == true ? $"[{i + 1}] {_currentSection.Locations[i].name}" : _currentSection.Locations[i].name;
						if (GUILayout.Button(buttonText, _defaultStyle) == true)
						{
							TeleportPlayer(_currentSection.Locations[i]);

							_showTeleports     = default;
							_currentSection    = default;
							_selectingLocation = default;

							break;
						}
					}

					GUILayout.EndVertical();
				}

				GUILayout.FlexibleSpace();
			}

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

		// PRIVATE METHODS

		private void Initialize()
		{
			if (_defaultStyle == null)
			{
				_defaultStyle = new GUIStyle(_skin.button);
				_defaultStyle.alignment = TextAnchor.MiddleCenter;

				if (Application.isMobilePlatform == true && Application.isEditor == false)
				{
					_defaultStyle.fontSize = 20;
					_defaultStyle.padding = new RectOffset(20, 20, 20, 20);
				}

				_selectedStyle = new GUIStyle(_defaultStyle);
				_selectedStyle.normal.textColor  = Color.green;
				_selectedStyle.focused.textColor = Color.green;
				_selectedStyle.hover.textColor   = Color.green;
			}
		}

		private void TeleportPlayer(Transform location)
		{
			Player player = GetLocalPlayer();
			if (player == null)
				return;

			KCC kcc = player.GetComponent<KCC>();
			if (kcc == null)
				return;

			kcc.TeleportRPC(location.position, kcc.FixedData.LookPitch, location.rotation.eulerAngles.y);
		}

		private float GetPlayerSpeed()
		{
			Player player = GetLocalPlayer();
			if (player == null)
				return 1.0f;

			return player.SpeedMultiplier;
		}

		private void TogglePlayerSpeed()
		{
			Player player = GetLocalPlayer();
			if (player == null)
				return;

			player.ToggleSpeedRPC(1);
		}

		private void ToggleFrameRate()
		{
			switch (_frameRateUpdater.TargetFrameRateStandalone)
			{
				case   0 : { _frameRateUpdater.TargetFrameRateStandalone =  30; break; }
				case  30 : { _frameRateUpdater.TargetFrameRateStandalone =  60; break; }
				case  60 : { _frameRateUpdater.TargetFrameRateStandalone =  90; break; }
				case  90 : { _frameRateUpdater.TargetFrameRateStandalone = 120; break; }
				case 120 : { _frameRateUpdater.TargetFrameRateStandalone = 144; break; }
				case 144 : { _frameRateUpdater.TargetFrameRateStandalone = 240; break; }
				case 240 : { _frameRateUpdater.TargetFrameRateStandalone = 288; break; }
				case 288 : { _frameRateUpdater.TargetFrameRateStandalone = 360; break; }
				case 360 : { _frameRateUpdater.TargetFrameRateStandalone = 432; break; }
				case 432 : { _frameRateUpdater.TargetFrameRateStandalone = 480; break; }
				case 480 : { _frameRateUpdater.TargetFrameRateStandalone = 576; break; }
				case 576 : { _frameRateUpdater.TargetFrameRateStandalone = 720; break; }
				case 720 : { _frameRateUpdater.TargetFrameRateStandalone =   0; break; }
				default:   { _frameRateUpdater.TargetFrameRateStandalone =   0; break; }
			}

			switch (_frameRateUpdater.TargetFrameRateMobile)
			{
				case   0 : { _frameRateUpdater.TargetFrameRateMobile =  30; break; }
				case  30 : { _frameRateUpdater.TargetFrameRateMobile =  60; break; }
				case  60 : { _frameRateUpdater.TargetFrameRateMobile =  90; break; }
				case  90 : { _frameRateUpdater.TargetFrameRateMobile = 120; break; }
				case 120 : { _frameRateUpdater.TargetFrameRateMobile =   0; break; }
				default:   { _frameRateUpdater.TargetFrameRateMobile =   0; break; }
			}
		}

		private void ToggleVSync()
		{
			QualitySettings.vSyncCount = QualitySettings.vSyncCount == 0 ? 1 : 0;
		}

		private float GetInputSmoothing()
		{
			Player player = GetLocalPlayer();
			if (player == null)
				return 0.0f;

			return player.Input.LookResponsivity;
		}

		private void ToggleInputSmoothing()
		{
			Player player = GetLocalPlayer();
			if (player == null)
				return;

			switch (player.Input.LookResponsivity)
			{
				case 0.000f : { player.Input.LookResponsivity = 0.005f; break; }
				case 0.005f : { player.Input.LookResponsivity = 0.010f; break; }
				case 0.010f : { player.Input.LookResponsivity = 0.015f; break; }
				case 0.015f : { player.Input.LookResponsivity = 0.020f; break; }
				case 0.020f : { player.Input.LookResponsivity = 0.025f; break; }
				case 0.025f : { player.Input.LookResponsivity = 0.035f; break; }
				case 0.035f : { player.Input.LookResponsivity = 0.050f; break; }
				case 0.050f : { player.Input.LookResponsivity = 0.075f; break; }
				case 0.075f : { player.Input.LookResponsivity = 0.100f; break; }
				case 0.100f : { player.Input.LookResponsivity = 0.000f; break; }
				default     : { player.Input.LookResponsivity = 0.000f; break; }
			}
		}

		private void ToggleFullScreen()
		{
			Resolution maxResolution            = default;
			int        maxResolutionSize        = default;
			int        maxResolutionRefreshRate = default;

			Resolution[] resolutions = Screen.resolutions;
			foreach (Resolution resolution in resolutions)
			{
				int resolutionSize = resolution.width * resolution.height;
				if (resolutionSize >= maxResolutionSize)
				{
					if (resolution.refreshRate >= maxResolutionRefreshRate)
					{
						maxResolutionSize        = resolutionSize;
						maxResolutionRefreshRate = resolution.refreshRate;
						maxResolution            = resolution;
					}
				}
			}

			switch (Screen.fullScreenMode)
			{
				case FullScreenMode.ExclusiveFullScreen: { Screen.SetResolution(maxResolution.width / 2, maxResolution.height / 2, FullScreenMode.Windowed,            maxResolution.refreshRate); break;}
				case FullScreenMode.FullScreenWindow:    { Screen.SetResolution(maxResolution.width,     maxResolution.height,     FullScreenMode.ExclusiveFullScreen, maxResolution.refreshRate); break;}
				case FullScreenMode.MaximizedWindow:     { Screen.SetResolution(maxResolution.width,     maxResolution.height,     FullScreenMode.FullScreenWindow,    maxResolution.refreshRate); break;}
				case FullScreenMode.Windowed:            { Screen.SetResolution(maxResolution.width,     maxResolution.height,     FullScreenMode.MaximizedWindow,     maxResolution.refreshRate); break;}
				default:
				{
					throw new NotImplementedException(Screen.fullScreenMode.ToString());
				}
			}
		}

		private void ToggleQualityLevel()
		{
			if (_sceneConfig == null)
				return;

			_sceneConfig.SetQualityLevel(_sceneConfig.GetQualityLevel() + 1);
		}

		private bool GetNumberDown(int offset)
		{
			switch (offset)
			{
				case 0 : { return Keyboard.current.numpad1Key.wasPressedThisFrame == true || Keyboard.current.digit1Key.wasPressedThisFrame == true; }
				case 1 : { return Keyboard.current.numpad2Key.wasPressedThisFrame == true || Keyboard.current.digit2Key.wasPressedThisFrame == true; }
				case 2 : { return Keyboard.current.numpad3Key.wasPressedThisFrame == true || Keyboard.current.digit3Key.wasPressedThisFrame == true; }
				case 3 : { return Keyboard.current.numpad4Key.wasPressedThisFrame == true || Keyboard.current.digit4Key.wasPressedThisFrame == true; }
				case 4 : { return Keyboard.current.numpad5Key.wasPressedThisFrame == true || Keyboard.current.digit5Key.wasPressedThisFrame == true; }
				case 5 : { return Keyboard.current.numpad6Key.wasPressedThisFrame == true || Keyboard.current.digit6Key.wasPressedThisFrame == true; }
				case 6 : { return Keyboard.current.numpad7Key.wasPressedThisFrame == true || Keyboard.current.digit7Key.wasPressedThisFrame == true; }
				case 7 : { return Keyboard.current.numpad8Key.wasPressedThisFrame == true || Keyboard.current.digit8Key.wasPressedThisFrame == true; }
				case 8 : { return Keyboard.current.numpad9Key.wasPressedThisFrame == true || Keyboard.current.digit9Key.wasPressedThisFrame == true; }
			}

			return false;
		}

		private Player GetLocalPlayer()
		{
			if (Runner == null)
				return default;

			PlayerRef localPlayerRef = Runner.LocalPlayer;
			if (localPlayerRef.IsValid == false)
				return default;

			if (_localPlayer == null)
			{
				_localPlayer = null;

				List<Player> players = Runner.SimulationUnityScene.GetComponents<Player>();
				for (int i = 0, count = players.Count; i < count; ++i)
				{
					Player player = players[i];
					if (player.Object != null && player.Object.InputAuthority == localPlayerRef)
					{
						_localPlayer = player;
						break;
					}
				}
			}

			return _localPlayer;
		}

		// DATA STRUCTURES

		[Serializable]
		private sealed class Section
		{
			public Transform   Root;
			public Transform[] Locations;
		}
	}
}
