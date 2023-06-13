namespace Example
{
	using UnityEngine;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	/// Advanced player implementation with first person view.
	/// </summary>
	[OrderBefore(typeof(KCC))]
	[OrderAfter(typeof(AdvancedPlayer))]
	public sealed class FirstPersonPlayer : AdvancedPlayer
	{
		// AdvancedPlayer INTERFACE

		protected override void OnSpawned()
		{
			// Disable visual for local player.
			Visual.SetVisibility(Object.HasInputAuthority == false);

			// Here we can show other visual (weapon, hands rig, ...).
		}

		// 1.
		protected override void ProcessEarlyFixedInput()
		{
			// Here we process input and set properties related to movement / look.
			// For following lines, we should use Input.FixedInput only. This property holds input for fixed updates.

			// Clamp input look rotation delta
			Vector2 lookRotation      = KCC.FixedData.GetLookRotation(true, true);
			Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, Input.FixedInput.LookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

			// Apply clamped look rotation delta
			KCC.AddLookRotation(lookRotationDelta);

			// Calculate input direction based on recently updated look rotation (the change propagates internally also to KCCData.TransformRotation)
			Vector3 inputDirection = KCC.FixedData.TransformRotation * new Vector3(Input.FixedInput.MoveDirection.x, 0.0f, Input.FixedInput.MoveDirection.y);

			KCC.SetInputDirection(inputDirection);

			if (Input.WasActivated(EGameplayInputAction.Jump) == true)
			{
				// By default the character jumps forward in facing direction
				Quaternion jumpRotation = KCC.FixedData.TransformRotation;

				if (inputDirection.IsAlmostZero() == false)
				{
					// If we are moving, jump in that direction instead
					jumpRotation = Quaternion.LookRotation(inputDirection);
				}

				// Applying jump impulse
				KCC.Jump(jumpRotation * JumpImpulse);
			}

			// Notice we are checking KCC.FixedData because we are in fixed update code path (render update uses KCC.RenderData)
			if (KCC.FixedData.IsGrounded == true)
			{
				// Sprint is updated only when grounded
				KCC.SetSprint(Input.FixedInput.Sprint);
			}

			if (Input.WasActivated(EGameplayInputAction.Dash) == true)
			{
				// Dash is movement related action, should be processed before KCC ticks.
				// We only care about registering processor to the KCC, responsibility for cleanup is on dash processor.
				KCC.AddModifier(DashProcessor);
			}

			// Another movement related actions here (crouch, ...)
		}

		// 2.
		protected override void OnFixedUpdate()
		{
			// Regular fixed update for Player/AdvancedPlayer class.
			// Executed after all player KCC updates and before HitboxManager.

			// Setting camera pivot rotation
			Vector2 pitchRotation = KCC.FixedData.GetLookRotation(true, false);
			CameraPivot.localRotation = Quaternion.Euler(pitchRotation);
		}

		// 3.
		protected override void ProcessLateFixedInput()
		{
			// Executed after HitboxManager. Process other non-movement actions like shooting.

			if (Input.WasActivated(EGameplayInputAction.LMB) == true)
			{
				// Left mouse button action
			}

			if (Input.WasActivated(EGameplayInputAction.RMB) == true)
			{
				// Right mouse button action
			}

			if (Input.WasActivated(EGameplayInputAction.MMB) == true)
			{
				// Middle mouse button action
			}
		}

		// 4.
		protected override void ProcessRenderInput()
		{
			// Here we process input and set properties related to movement / look.
			// For following lines, we should use Input.RenderInput and Input.CachedInput only. These properties hold input for render updates.
			// Input.RenderInput holds input for current render frame.
			// Input.CachedInput holds combined input for all render frames from last fixed update. This property will be used to set input for next fixed update (polled by Fusion).

			// Get look rotation from last fixed update (not last render!)
			Vector2 lookRotation = KCC.FixedData.GetLookRotation(true, true);

			// For correct look rotation, we have to apply deltas from all render frames since last fixed update => stored in Input.CachedInput
			Vector2 lookRotationDelta = KCCUtility.GetClampedLookRotationDelta(lookRotation, Input.CachedInput.LookRotationDelta, -MaxCameraAngle, MaxCameraAngle);

			KCC.SetLookRotation(lookRotation + lookRotationDelta);

			Vector3 inputDirection = default;

			// MoveDirection values from previous render frames are already consumed and applied by KCC, so we use Input.RenderInput (non-accumulated input for this frame)
			Vector3 moveDirection = Input.RenderInput.MoveDirection.X0Y();
			if (moveDirection.IsZero() == false)
			{
				inputDirection = KCC.RenderData.TransformRotation * moveDirection;
			}

			KCC.SetInputDirection(inputDirection);

			// Jump is predicted for render as well.
			if (Input.WasActivated(EGameplayInputAction.Jump) == true)
			{
				// By default the character jumps forward in facing direction
				Quaternion jumpRotation = KCC.RenderData.TransformRotation;

				if (inputDirection.IsZero() == false)
				{
					// If we are moving, jump in that direction instead
					jumpRotation = Quaternion.LookRotation(inputDirection);
				}

				KCC.Jump(jumpRotation * JumpImpulse);
			}

			// Notice we are checking KCC.RenderData because we are in render update code path (fixed update uses KCC.FixedData)
			if (KCC.RenderData.IsGrounded == true)
			{
				// Sprint is updated only when grounded
				KCC.SetSprint(Input.CachedInput.Sprint);
			}

			// At his point, KCC haven't been updated yet (except look rotation, which propagates immediately) so camera have to be synced later.
			// Because this is advanced implementation, base class triggers manual KCC update immediately after this method.
			// This allows us to synchronize camera in OnEarlyRender(). To keep consistency with fixed update, camera related properties are updated in regular render update - OnRender().
		}

		// 5.
		protected override void OnRender()
		{
			// Regular render update

			// For render we care only about input authority.
			// This can be extended to state authority if needed (inner code won't be executed on host for other players, having camera pivots to be set only from fixed update, causing jitter if spectating that player)
			if (Object.HasInputAuthority == true)
			{
				Vector2 pitchRotation = KCC.RenderData.GetLookRotation(true, false);
				CameraPivot.localRotation = Quaternion.Euler(pitchRotation);
			}
		}

		// 6.
		// Late processing of render input is out of scope of this example.
		// This would involve extra complexity.
	}
}
