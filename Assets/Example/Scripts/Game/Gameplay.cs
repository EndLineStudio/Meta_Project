namespace Example
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Fusion;

	/// <summary>
	/// Main entry point for gameplay logic and spawning players.
	/// </summary>
	[RequireComponent(typeof(NetworkRunner))]
	[RequireComponent(typeof(NetworkEvents))]
	public sealed class Gameplay : MonoBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private NetworkObject _defaultPlayer;
		[SerializeField]
		private NetworkObject _vrPlayer;

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			NetworkEvents events = GetComponent<NetworkEvents>();
			events.OnConnectedToServer.AddListener(OnClientConnectedToServer);
			events.OnReliableData.AddListener(OnDataReceived);
			events.PlayerJoined.AddListener(OnPlayerJoined);
			events.PlayerLeft.AddListener(OnPlayerLeft);
		}

		// PRIVATE METHODS

		private PlayerConfig GetLocalPlayerConfig()
		{
			PlayerConfig playerConfig = new PlayerConfig();
			playerConfig.Platform = ApplicationUtility.IsVREnabled() == true ? 1 : 0;

			return playerConfig;
		}

		private void SpawnPlayer(NetworkRunner runner, PlayerRef playerRef, PlayerConfig playerConfig)
		{
			bool isVRPlayer = playerConfig.Platform == 1;

			// Random spawnpoint lookup and spawn

			List<SpawnPoint> spawnPoints = runner.SimulationUnityScene.GetComponents<SpawnPoint>();
			Transform        spawnPoint  = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].transform;

			NetworkObject player = runner.Spawn(isVRPlayer == true ? _vrPlayer : _defaultPlayer, spawnPoint.position, spawnPoint.rotation, playerRef);

			// Set player instance as PlayerObject so we can easily get it from other locations
			runner.SetPlayerObject(playerRef, player);

			// Player must be always interested to his NetworkObject to prevent getting out of AoI (typically teleporting after setting AoI position)
			runner.SetPlayerAlwaysInterested(playerRef, player, true);
		}

		private void OnClientConnectedToServer(NetworkRunner runner)
		{
			// In Shared mode spawn is handled from OnPlayerJoined
			if (runner.GameMode == GameMode.Shared)
				return;

			// Otherwise send player config to the server
			PlayerConfig playerConfig = GetLocalPlayerConfig();

			runner.SendReliableDataToServer(PlayerConfig.Serialize(playerConfig));
		}

		private void OnDataReceived(NetworkRunner runner, PlayerRef playerRef, ArraySegment<byte> data)
		{
			if (runner.IsServer == false)
				return;

			PlayerConfig playerConfig = PlayerConfig.Deserialize(data.Array, data.Offset, data.Count);
			if (playerConfig == null)
				return;

			SpawnPlayer(runner, playerRef, playerConfig);
		}

		private void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
		{
			if (runner.LocalPlayer == playerRef && (runner.IsServer == true || runner.GameMode == GameMode.Shared))
			{
				PlayerConfig playerConfig = GetLocalPlayerConfig();
				SpawnPlayer(runner, playerRef, playerConfig);
			}
		}

		private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
		{
			if (runner.IsServer == false && runner.IsSharedModeMasterClient == false)
				return;

			foreach (Player player in runner.GetAllBehaviours<Player>())
			{
				if (player.Object.InputAuthority == playerRef)
				{
					StartCoroutine(DespawnPlayerCoroutine(runner, player.Object));
					return;
				}
			}

			foreach (VRPlayer player in runner.GetAllBehaviours<VRPlayer>())
			{
				if (player.Object.InputAuthority == playerRef)
				{
					StartCoroutine(DespawnPlayerCoroutine(runner, player.Object));
					return;
				}
			}
		}

		private IEnumerator DespawnPlayerCoroutine(NetworkRunner runner, NetworkObject player)
		{
			player.RequestStateAuthority();

			while (player != null && player.HasStateAuthority == false)
				yield return null;

			if (runner != null && player != null)
			{
				runner.Despawn(player);
			}
		}

		// DATA STRUCTURES

		private sealed class PlayerConfig
		{
			public int Platform;

			public static byte[] Serialize(PlayerConfig playerConfig)
			{
				return System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(playerConfig));
			}

			public static PlayerConfig Deserialize(byte[] data, int index, int count)
			{
				try
				{
					return JsonUtility.FromJson<PlayerConfig>(System.Text.Encoding.UTF8.GetString(data, index, count));
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
