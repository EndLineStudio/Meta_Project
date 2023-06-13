using UnityEditor;

[CustomEditor(typeof(RPMPlayerRefs))]
public class RPMPlayerRefsEditor : Editor
{
    private SerializedProperty playerNameKey;
    private SerializedProperty playerNameLength;
    private SerializedProperty genderKey;
    private SerializedProperty localAvatarKey;
    private SerializedProperty runtimeAvatarKey;
    private SerializedProperty runtimeAvatarUrlLength;
    private SerializedProperty openUrl;
    private SerializedProperty validationStart;
    private SerializedProperty validationContain;
    private SerializedProperty validationEnd;
    private SerializedProperty roomNameKey;
    private SerializedProperty roomNameLength;
    private SerializedProperty regionNameKey;

    private void OnEnable()
    {
        playerNameKey = serializedObject.FindProperty("playerNameKey");
        playerNameLength = serializedObject.FindProperty("playerNameLength");
        genderKey = serializedObject.FindProperty("genderKey");
        localAvatarKey = serializedObject.FindProperty("localAvatarKey");
        runtimeAvatarKey = serializedObject.FindProperty("runtimeAvatarKey");
        runtimeAvatarUrlLength = serializedObject.FindProperty("runtimeAvatarUrlLength");
        openUrl = serializedObject.FindProperty("openUrl");
        validationStart = serializedObject.FindProperty("validationStart");
        validationContain = serializedObject.FindProperty("validationContain");
        validationEnd = serializedObject.FindProperty("validationEnd");
        roomNameKey = serializedObject.FindProperty("roomNameKey");
        roomNameLength = serializedObject.FindProperty("roomNameLength");
        regionNameKey = serializedObject.FindProperty("regionNameKey");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("PlayerPrefs Key", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(playerNameKey);
        EditorGUILayout.PropertyField(genderKey);
        EditorGUILayout.PropertyField(localAvatarKey);
        EditorGUILayout.PropertyField(runtimeAvatarKey);
        EditorGUILayout.PropertyField(roomNameKey);
        EditorGUILayout.PropertyField(regionNameKey);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Validator", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Char Length", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(playerNameLength);
        EditorGUILayout.PropertyField(runtimeAvatarUrlLength);
        EditorGUILayout.PropertyField(roomNameLength);
        EditorGUILayout.LabelField("URL", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(validationStart);
        EditorGUILayout.PropertyField(validationContain);
        EditorGUILayout.PropertyField(validationEnd);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create Avatar Links", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(openUrl);

        serializedObject.ApplyModifiedProperties();
    }
}
