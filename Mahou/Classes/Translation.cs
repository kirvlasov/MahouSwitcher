using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace Mahou
{
    enum UiText
    {
        AutostartWithWindows,
        Hotkeys,
        ConvertWord,
        ConvertSelection,
        ConvertLine,
        CsSwitch,
        RePress,
        ReSelect,
        SwitchLayoutByKey,
        BlockCtrl,
        TrayIcon,
        CycleMode,
        Emu,
        SwitchBetweenLayouts,
        Language,
        Apply,
        Ok,
        Cancel,
        Help,
        UseSpecificLayoutChangingByLeftRightCtrls,
        LCtrlSwitchesTo,
        RCtrlSwitchesTo,
        MoreConfigs,
        SymbolIgnore,
        MoreTries,
        TrayShowHide,
        TrayExit,
        TrayDescription,
        DisplayLanguage,
        RefreshRateMs,
        Colors,
        Font,
        Size,
        Position,
        More,
        Back,
        DoubleHotkey,
        Delay,
        ExperimentalCsSwitchPlus,
        TransparentBackgroundInLanguageTooltip,
        UseSnippets,
        OnChange,
        HighlightScrollLockWhenLanguage1Active,
        ConvertMultipleWordsHotkey
    }

    enum ToolTipText
    {
        CycleMode,
        ConvertWordHotkey,
        ConvertSelectionHotkey,
        ConvertLineHotkey,
        TrayIcon,
        BlockCtrl,
        SwitchLayoutKeys,
        EmulateLayoutSwitch,
        CsSwitch,
        SwitchBetweenLayouts,
        RePress,
        EatOneSpace,
        ReSelect,
        EmuType,
        LCtrlSpecificLayout,
        RCtrlSpecificLayout,
        UseSpecificLayoutChanging,
        EmuTypeTitle,
        SymbolIgnore,
        MoreTries,
        DisplayLanguage,
        RefreshRate,
        Colors,
        Size,
        Position,
        DoubleHotkey,
        DoubleHotkeyDelay,
        ExperimentalCsSwitchPlus,
        TransparentBackground,
        UseSnippets,
        OnChange,
        ScrollLockHighlight,
        ConvertMultipleWords
    }

    enum MessageText
    {
        Help,
        AttentionTitle,
        DuplicateConvertWordLineHotkeys,
        WarningTitle,
        ConvertWordHotkeyModifiersOnly,
        ConvertSelectionHotkeyModifiersOnly,
        ConvertLineHotkeyModifiersOnly,
        SelectedLocalesRemoved,
        SnippetsConfiguredWrong,
        SnippetsErrorTitle,
        CapsLockSwitchOnlyWarning,
        CapsLockWarningTitle
    }

    static class Translation
    {
        private const string DefaultLanguageCode = "EN";
        private static readonly ResourceManager Strings = new ResourceManager("Mahou.Properties.Strings", typeof(Translation).Assembly);
        private static readonly string[] SupportedLanguageCodesInternal = { "EN", "RU" };
        private static CultureInfo culture = CultureInfo.GetCultureInfo("en");

        public static IReadOnlyList<string> SupportedLanguageCodes {
            get { return SupportedLanguageCodesInternal; }
        }

        public static string CurrentLanguageCode { get; private set; } = DefaultLanguageCode;

        public static void SetLanguage(string languageCode)
        {
            CurrentLanguageCode = NormalizeLanguageCode(languageCode);
            culture = CultureFor(CurrentLanguageCode);
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public static string GetNextLanguage(string languageCode)
        {
            var current = NormalizeLanguageCode(languageCode);
            for (int i = 0; i < SupportedLanguageCodesInternal.Length; i++) {
                if (SupportedLanguageCodesInternal[i] == current) {
                    return SupportedLanguageCodesInternal[(i + 1) % SupportedLanguageCodesInternal.Length];
                }
            }
            return DefaultLanguageCode;
        }

        public static string NormalizeLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode)) {
                return DefaultLanguageCode;
            }

            foreach (var supportedLanguageCode in SupportedLanguageCodesInternal) {
                if (string.Equals(languageCode, supportedLanguageCode, System.StringComparison.OrdinalIgnoreCase)) {
                    return supportedLanguageCode;
                }
            }

            return DefaultLanguageCode;
        }

        public static string UI(UiText text)
        {
            return GetString("UI_" + text);
        }

        public static string ToolTip(ToolTipText text)
        {
            return GetString("ToolTip_" + text);
        }

        public static string Message(MessageText text)
        {
            return GetString("Message_" + text);
        }

        private static string GetString(string key)
        {
            return Strings.GetString(key, culture) ?? string.Empty;
        }

        private static CultureInfo CultureFor(string languageCode)
        {
            if (languageCode == "RU") {
                return CultureInfo.GetCultureInfo("ru");
            }

            return CultureInfo.GetCultureInfo("en");
        }
    }
}
