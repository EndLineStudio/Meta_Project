namespace Fusion.Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example of simplified movement processor. This imlementation doesn't suffer from position error accumulation due to render delta time integration.
	/// In render update, all properties (speed, velocities, ...) are calculated with fixed delta time. Render delta time is used when calculating actual position delta.
	/// Results of this processor in Fixed/Render updates should always be in sync.
	/// </summary>
	public partial class SimpleMovementKCCProcessor : BaseKCCProcessor, IGroundKCCProcessor, IAirKCCProcessor
	{
		// CONSTANTS

		public static readonly int DefaultPriority = 500;

		// PRIVATE MEMBERS

		[SerializeField][Tooltip("Maximum allowed speed the KCC can move with player input.")]
		private float _kinematicSpeed = 8.0f;
		[SerializeField][Tooltip("Custom jump multiplier.")]
		private float _jumpMultiplier = 1.0f;
		[SerializeField][Tooltip("Custom gravity multiplier.")]
		private float _gravityMultiplier = 1.0f;
		[SerializeField][Tooltip("Dynamic velocity is decelerated by actual dynamic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
		private float _dynamicGroundFriction = 20.0f;
		[SerializeField][Tooltip("Kinematic velocity is accelerated by calculated kinematic speed multiplied by this.")]
		private float _kinematicGroundAcceleration = 50.0f;
		[SerializeField][Tooltip("Kinematic velocity is decelerated by actual kinematic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
		private float _kinematicGroundFriction = 35.0f;
		[SerializeField][Tooltip("Dynamic velocity is decelerated by actual dynamic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
		private float _dynamicAirFriction = 2.0f;
		[SerializeField][Tooltip("Kinematic velocity is accelerated by calculated kinematic speed multiplied by this.")]
		private float _kinematicAirAcceleration = 5.0f;
		[SerializeField][Tooltip("Kinematic velocity is decelerated by actual kinematic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
		private float _kinematicAirFriction = 2.0f;

		// BaseKCCProcessor INTERFACE

		public override float Priority => DefaultPriority;

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			EKCCStages stages = default;

			stages |= EKCCStages.SetInputProperties;
			stages |= EKCCStages.SetDynamicVelocity;
			stages |= EKCCStages.SetKinematicDirection;
			stages |= EKCCStages.SetKinematicTangent;
			stages |= EKCCStages.SetKinematicSpeed;
			stages |= EKCCStages.SetKinematicVelocity;
			stages |= EKCCStages.ProcessPhysicsQuery;
			stages |= EKCCStages.OnStay;

			return stages;
		}

		public override void SetInputProperties(KCC kcc, KCCData data)
		{
			data.Gravity *= _gravityMultiplier;

			SuppressOtherProcessors(kcc);
		}

		public override void SetDynamicVelocity(KCC kcc, KCCData data)
		{
			KCCData fixedData       = kcc.FixedData;
			float   fixedDeltaTime  = fixedData.DeltaTime;
			Vector3 dynamicVelocity = fixedData.DynamicVelocity;

			// Next fixed/render value is based on values from last fixed update.
			// This is the reason why fixedData is sometimes used instead of data (except values calculated in recently executed processor chain, these are safe to use).

			if (fixedData.IsGrounded == false || (fixedData.IsSteppingUp == false && (fixedData.IsSnappingToGround == true || fixedData.GroundDistance > 0.001f)))
			{
				dynamicVelocity += data.Gravity * fixedDeltaTime;
			}

			if (fixedData.IsGrounded == true && data.JumpImpulse.IsZero() == false && _jumpMultiplier > 0.0f)
			{
				Vector3 jumpDirection = data.JumpImpulse.normalized;

				dynamicVelocity -= Vector3.Scale(dynamicVelocity, jumpDirection);
				dynamicVelocity += (data.JumpImpulse / kcc.Settings.Mass) * _jumpMultiplier;

				data.HasJumped = true;
			}

			dynamicVelocity += data.ExternalVelocity;
			dynamicVelocity += data.ExternalAcceleration * fixedDeltaTime;
			dynamicVelocity += (data.ExternalImpulse / kcc.Settings.Mass);
			dynamicVelocity += (data.ExternalForce / kcc.Settings.Mass) * fixedDeltaTime;

			if (dynamicVelocity.IsZero() == false)
			{
				if (dynamicVelocity.IsAlmostZero(0.001f) == true)
				{
					dynamicVelocity = default;
				}
				else
				{
					if (fixedData.IsGrounded == true)
					{
						Vector3 frictionAxis = Vector3.one;
						if (fixedData.GroundDistance > 0.001f || fixedData.IsSnappingToGround == true)
						{
							frictionAxis.y = default;
						}

						dynamicVelocity += KCCPhysicsUtility.GetFriction(dynamicVelocity, dynamicVelocity, frictionAxis, fixedData.GroundNormal, fixedData.KinematicSpeed, true, 0.0f, 0.0f, _dynamicGroundFriction, fixedDeltaTime, fixedDeltaTime);
					}
					else
					{
						dynamicVelocity += KCCPhysicsUtility.GetFriction(dynamicVelocity, dynamicVelocity, new Vector3(1.0f, 0.0f, 1.0f), fixedData.KinematicSpeed, true, 0.0f, 0.0f, _dynamicAirFriction, fixedDeltaTime, fixedDeltaTime);
					}
				}
			}

			data.DynamicVelocity = dynamicVelocity;

			SuppressOtherProcessors(kcc);
		}

		public override void SetKinematicDirection(KCC kcc, KCCData data)
		{
			data.KinematicDirection = data.InputDirection.OnlyXZ();

			SuppressOtherProcessors(kcc);
		}

		public override void SetKinematicTangent(KCC kcc, KCCData data)
		{
			KCCData fixedData = kcc.FixedData;

			if (fixedData.IsGrounded == true)
			{
				if (data.KinematicDirection.IsAlmostZero(0.0001f) == false && KCCPhysicsUtility.ProjectOnGround(fixedData.GroundNormal, data.KinematicDirection, out Vector3 projectedMoveDirection) == true)
				{
					data.KinematicTangent = projectedMoveDirection.normalized;
				}
				else
				{
					data.KinematicTangent = fixedData.GroundTangent;
				}
			}
			else
			{
				if (data.KinematicDirection.IsAlmostZero(0.0001f) == false)
				{
					data.KinematicTangent = data.KinematicDirection.normalized;
				}
				else
				{
					data.KinematicTangent = data.TransformDirection;
				}
			}

			SuppressOtherProcessors(kcc);
		}

		public override void SetKinematicSpeed(KCC kcc, KCCData data)
		{
			data.KinematicSpeed = _kinematicSpeed;

			SuppressOtherProcessors(kcc);
		}

		public override void SetKinematicVelocity(KCC kcc, KCCData data)
		{
			KCCData fixedData         = kcc.FixedData;
			float   fixedDeltaTime    = fixedData.DeltaTime;
			Vector3 kinematicVelocity = fixedData.KinematicVelocity;

			if (fixedData.IsGrounded == true)
			{
				if (kinematicVelocity.IsAlmostZero() == false && KCCPhysicsUtility.ProjectOnGround(fixedData.GroundNormal, kinematicVelocity, out Vector3 projectedKinematicVelocity) == true)
				{
					kinematicVelocity = projectedKinematicVelocity.normalized * kinematicVelocity.magnitude;
				}

				if (data.KinematicDirection.IsAlmostZero() == true)
				{
					data.KinematicVelocity = kinematicVelocity + KCCPhysicsUtility.GetFriction(kinematicVelocity, kinematicVelocity, Vector3.one, fixedData.GroundNormal, data.KinematicSpeed, true, 0.0f, 0.0f, _kinematicGroundFriction, fixedDeltaTime, fixedDeltaTime);
					SuppressOtherProcessors(kcc);
					return;
				}
			}
			else
			{
				if (data.KinematicDirection.IsAlmostZero() == true)
				{
					data.KinematicVelocity = kinematicVelocity + KCCPhysicsUtility.GetFriction(kinematicVelocity, kinematicVelocity, new Vector3(1.0f, 0.0f, 1.0f), data.KinematicSpeed, true, 0.0f, 0.0f, _kinematicAirFriction, fixedDeltaTime, fixedDeltaTime);
					SuppressOtherProcessors(kcc);
					return;
				}
			}

			Vector3 moveDirection = kinematicVelocity;
			if (moveDirection.IsZero() == true)
			{
				moveDirection = data.KinematicTangent;
			}

			Vector3 acceleration;
			Vector3 friction;

			if (fixedData.IsGrounded == true)
			{
				acceleration = KCCPhysicsUtility.GetAcceleration(kinematicVelocity, data.KinematicTangent, Vector3.one, data.KinematicSpeed, false, data.KinematicDirection.magnitude, 0.0f, _kinematicGroundAcceleration, 0.0f, fixedDeltaTime, fixedDeltaTime);
				friction     = KCCPhysicsUtility.GetFriction(kinematicVelocity, moveDirection, Vector3.one, fixedData.GroundNormal, data.KinematicSpeed, false, 0.0f, 0.0f, _kinematicGroundFriction, fixedDeltaTime, fixedDeltaTime);
			}
			else
			{
				acceleration = KCCPhysicsUtility.GetAcceleration(kinematicVelocity, data.KinematicTangent, Vector3.one, data.KinematicSpeed, false, data.KinematicDirection.magnitude, 0.0f, _kinematicAirAcceleration, 0.0f, fixedDeltaTime, fixedDeltaTime);
				friction     = KCCPhysicsUtility.GetFriction(kinematicVelocity, moveDirection, new Vector3(1.0f, 0.0f, 1.0f), data.KinematicSpeed, false, 0.0f, 0.0f, _kinematicAirFriction, fixedDeltaTime, fixedDeltaTime);
			}

			kinematicVelocity = KCCPhysicsUtility.CombineAccelerationAndFriction(kinematicVelocity, acceleration, friction);

			if (kinematicVelocity.sqrMagnitude > data.KinematicSpeed * data.KinematicSpeed)
			{
				kinematicVelocity = kinematicVelocity / Vector3.Magnitude(kinematicVelocity) * data.KinematicSpeed;
			}

			if (data.HasJumped == true && kinematicVelocity.y < 0.0f)
			{
				kinematicVelocity.y = 0.0f;
			}

			data.KinematicVelocity = kinematicVelocity;

			SuppressOtherProcessors(kcc);
		}

		public override void ProcessPhysicsQuery(KCC kcc, KCCData data)
		{
			KCCData fixedData = kcc.FixedData;

			if (data.IsGrounded == true)
			{
				if (fixedData.WasGrounded == true && data.IsSnappingToGround == false && data.DynamicVelocity.y < 0.0f && data.DynamicVelocity.OnlyXZ().IsAlmostZero() == true)
				{
					data.DynamicVelocity.y = 0.0f;
				}

				if (fixedData.WasGrounded == false)
				{
					if (data.KinematicVelocity.OnlyXZ().IsAlmostZero() == true)
					{
						data.KinematicVelocity.y = 0.0f;
					}
					else
					{
						if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.KinematicVelocity, out Vector3 projectedKinematicVelocity) == true)
						{
							data.KinematicVelocity = projectedKinematicVelocity.normalized * data.KinematicVelocity.magnitude;
						}
					}
				}
			}
			else
			{
				if (fixedData.WasGrounded == false && data.DynamicVelocity.y > 0.0f && data.DeltaTime > 0.0f)
				{
					Vector3 currentVelocity;

					if (kcc.IsInFixedUpdate == true)
					{
						currentVelocity = (data.TargetPosition - data.BasePosition) / data.DeltaTime;
					}
					else
					{
						currentVelocity = (data.TargetPosition - fixedData.TargetPosition) / (fixedData.DeltaTime * data.Alpha);
					}

					if (currentVelocity.y.IsAlmostZero() == true)
					{
						data.DynamicVelocity.y = 0.0f;
					}
				}
			}

			SuppressOtherProcessors(kcc);
		}

		public override void OnStay(KCC kcc, KCCData data)
		{
			if (kcc.IsInFixedUpdate == false)
			{
				kcc.TransientData.Clear();
			}
		}

		// PRIVATE METHODS

		private static void SuppressOtherProcessors(KCC kcc)
		{
			kcc.SuppressProcessors<IGroundKCCProcessor>();
			kcc.SuppressProcessors<IAirKCCProcessor>();
		}
	}
}
