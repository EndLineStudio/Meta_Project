namespace Example
{
	using UnityEngine;
	using Fusion.KCC;

	/// <summary>
	/// Example processor - logging stage execution to console.
	/// </summary>
	public sealed class LogKCCProcessor : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private EKCCStages _stages = EKCCStages.All;

		// KCCProcessor INTERFACE

		public override EKCCStages GetValidStages(KCC kcc, KCCData data)
		{
			return _stages;
		}

		public override void SetInputProperties   (KCC kcc, KCCData data)                  { Log(kcc, data, $"SetInputProperties");                }
		public override void SetDynamicVelocity   (KCC kcc, KCCData data)                  { Log(kcc, data, $"SetDynamicVelocity");                }
		public override void SetKinematicDirection(KCC kcc, KCCData data)                  { Log(kcc, data, $"SetKinematicDirection");             }
		public override void SetKinematicTangent  (KCC kcc, KCCData data)                  { Log(kcc, data, $"SetKinematicTangent");               }
		public override void SetKinematicSpeed    (KCC kcc, KCCData data)                  { Log(kcc, data, $"SetKinematicSpeed");                 }
		public override void SetKinematicVelocity (KCC kcc, KCCData data)                  { Log(kcc, data, $"SetKinematicVelocity");              }
		public override void ProcessPhysicsQuery  (KCC kcc, KCCData data)                  { Log(kcc, data, $"ProcessPhysicsQuery");               }
		public override void OnEnter              (KCC kcc, KCCData data)                  { Log(kcc, data, $"OnEnter");                           }
		public override void OnExit               (KCC kcc, KCCData data)                  { Log(kcc, data, $"OnExit");                            }
		public override void OnStay               (KCC kcc, KCCData data)                  { Log(kcc, data, $"OnStay");                            }
		public override void OnInterpolate        (KCC kcc, KCCData data)                  { Log(kcc, data, $"OnInterpolate");                     }
		public override void ProcessUserLogic     (KCC kcc, KCCData data, object userData) { Log(kcc, data, $"ProcessUserLogic + {{{userData}}}"); }

		// PRIVATE METHODS

		private static void Log(KCC kcc, KCCData data, string message)
		{
			string logMessage = $"[{Time.frameCount}][{data.Tick}][{kcc.name}] {message}";

			if (kcc.IsInFixedUpdate == true)
			{
				Debug.LogWarning(logMessage);
			}
			else
			{
				Debug.Log(logMessage);
			}
		}
	}
}
