namespace Fusion.KCC
{
	/// <summary>
	/// Partial implementation of KCCSettings class - use this to extend with your own settings.
	/// This class doesn't support rollback, but is compatible with pooling.
	/// </summary>
	public partial class KCCSettings
	{
		// PUBLIC MEMBERS

		// Put your properties here

		// PARTIAL METHODS

		partial void CopyUserSettingsFromOther(KCCSettings other)
		{
			// Make a deep copy of your properties.
			// This method is also executed on (de)initialization to restore from backup.
		}
	}
}
