using System.Windows.Forms;

namespace KioskWin.Core;

public static class KioskUnlockPolicy
{
    public static bool ShouldUnlockDirectly(Keys keyData, KioskConfig config)
    {
        var adminCombo = KeyCombinationParser.Parse(config.AdminKeyCombination);
        return adminCombo != Keys.None &&
            keyData == adminCombo &&
            !HasConfiguredAdminPassword(config);
    }

    public static bool HasConfiguredAdminPassword(KioskConfig config)
    {
        return !string.IsNullOrWhiteSpace(config.AdminPasswordHash) &&
            !string.IsNullOrWhiteSpace(config.PasswordSalt);
    }
}
