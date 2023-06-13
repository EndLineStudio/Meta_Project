namespace Example
{
	using UnityEngine;
	using UnityEngine.XR;

	using XRInputDevice  = UnityEngine.XR.InputDevice;
	using XRCommonUsages = UnityEngine.XR.CommonUsages;

	public struct VRPose
	{
		// PUBLIC MEMBERS

		public Vector3    HeadPosition;
		public Quaternion HeadRotation;
		public Vector3    LeftHandPosition;
		public Quaternion LeftHandRotation;
		public Vector3    RightHandPosition;
		public Quaternion RightHandRotation;

		// PUBLIC METHODS

		public static VRPose Get()
		{
			VRPose pose = new VRPose();

			XRInputDevice head      = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
			XRInputDevice leftHand  = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			XRInputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

			if (     head.isValid == true &&      head.TryGetFeatureValue(XRCommonUsages.devicePosition, out Vector3      headPosition) == true) { pose.HeadPosition      =      headPosition; }
			if ( leftHand.isValid == true &&  leftHand.TryGetFeatureValue(XRCommonUsages.devicePosition, out Vector3  leftHandPosition) == true) { pose.LeftHandPosition  =  leftHandPosition; }
			if (rightHand.isValid == true && rightHand.TryGetFeatureValue(XRCommonUsages.devicePosition, out Vector3 rightHandPosition) == true) { pose.RightHandPosition = rightHandPosition; }

			if (     head.isValid == true &&      head.TryGetFeatureValue(XRCommonUsages.deviceRotation, out Quaternion      headRotation) == true) { pose.HeadRotation      =      headRotation; }
			if ( leftHand.isValid == true &&  leftHand.TryGetFeatureValue(XRCommonUsages.deviceRotation, out Quaternion  leftHandRotation) == true) { pose.LeftHandRotation  =  leftHandRotation; }
			if (rightHand.isValid == true && rightHand.TryGetFeatureValue(XRCommonUsages.deviceRotation, out Quaternion rightHandRotation) == true) { pose.RightHandRotation = rightHandRotation; }

			return pose;
		}
	}
}
