using UnityEditor;

[CustomEditor(typeof(RPMAvatarRefs))]
public class RPMAvatarRefsEditor : Editor
{
    private SerializedProperty masculineAvatarObject;
    private SerializedProperty masculineAvatar;
    private SerializedProperty feminineAvatarObject;
    private SerializedProperty feminineAvatar;

    private void OnEnable()
    {
        masculineAvatarObject = serializedObject.FindProperty("masculineAvatarObject");
        masculineAvatar = serializedObject.FindProperty("masculineAvatar");
        feminineAvatarObject = serializedObject.FindProperty("feminineAvatarObject");
        feminineAvatar = serializedObject.FindProperty("feminineAvatar");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Masculine Reference", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(masculineAvatarObject);
        EditorGUILayout.PropertyField(masculineAvatar);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Feminine Reference", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(feminineAvatarObject);
        EditorGUILayout.PropertyField(feminineAvatar);

        serializedObject.ApplyModifiedProperties();
    }
}
