using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using NLog;

namespace Mahou {
	class MMain {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		#region DLLs
		[DllImport("user32.dll")]
		public static extern uint RegisterWindowMessage(string message);
		#endregion
		#region Prevent another instance variables
		public const string appGUid = "ec511418-1d57-4dbe-a0c3-c6022b33735b";
		public static uint ao = RegisterWindowMessage("AlderyOpenedMahou!");
		#endregion
		#region All Main variables, arrays etc.
		public static List<KMHook.YuKey> c_word = new List<KMHook.YuKey>();
		public static List<List<KMHook.YuKey>> c_words = new List<List<KMHook.YuKey>>();
		public static IntPtr _hookID = IntPtr.Zero;
		public static IntPtr _mouse_hookID = IntPtr.Zero;
		public static KMHook.LowLevelProc _proc = KMHook.HookCallback;
		public static KMHook.LowLevelProc _mouse_proc = KMHook.MouseHookCallback;
		public static Locales.Locale[] locales = Locales.AllList();
		public static Configs MyConfs = new Configs();
		public static MahouForm mahou;
		public static List<string> lcnmid = new List<string>();
		#endregion
		[STAThread] //DO NOT REMOVE THIS
		public static void Main(string[] args) {
			LogHelper.ConfigureNlog();
			log.Trace("Program start");
			using(var mutex = new Mutex(false, "Local\\" + appGUid)) {
				log.Trace("Mutex created");
				if(!mutex.WaitOne(0, false)) {
					KMHook.PostMessage((IntPtr)0xffff, ao, 0, 0);
					return;
				}
				if(locales.Length < 2) {
					Locales.IfLessThan2();
				} else {
					Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
					Application.EnableVisualStyles();
					Application.SetDefaultFont(new Font("Microsoft Sans Serif", 8.25f));
					Application.SetCompatibleTextRenderingDefault(false);
					mahou = new MahouForm();
					InitLanguage();
					//Refreshes icon text language at startup
					mahou.icon.RefreshText(Translation.UI(UiText.TrayDescription), Translation.UI(UiText.TrayShowHide), Translation.UI(UiText.TrayExit));
					KMHook.ReInitSnippets();
					StartHook();
					//for first run, add your locale 1 & locale 2 to settings
					if(MyConfs.Read("Locales", "locale1Lang") == "" && MyConfs.Read("Locales", "locale2Lang") == "") {
						MyConfs.Write("Locales", "locale1uId", locales[0].uId.ToString());
						MyConfs.Write("Locales", "locale2uId", locales[1].uId.ToString());
						MyConfs.Write("Locales", "locale1Lang", locales[0].Lang);
						MyConfs.Write("Locales", "locale2Lang", locales[1].Lang);
					}
					try {
						Application.Run();

					} catch(Exception ex) {
						log.Fatal(ex, "Global error handler caught the exception in app");
					}
					StopHook();
				}
			}
		}
		public static void InitLanguage() {
			var languageCode = Translation.NormalizeLanguageCode(MyConfs.Read("Locales", "LANGUAGE"));
			if(languageCode != MyConfs.Read("Locales", "LANGUAGE")) {
				MyConfs.Write("Locales", "LANGUAGE", languageCode);
			}
			Translation.SetLanguage(languageCode);
		}
		#region Actions with hooks
		public static void StartHook() {
			if(!CheckHook()) {
				return;
			}
			log.Trace("Set hook");
			_mouse_hookID = KMHook.SetHook(_mouse_proc, (int)KMHook.KMMessages.WH_MOUSE_LL);
			_hookID = KMHook.SetHook(_proc, (int)KMHook.KMMessages.WH_KEYBOARD_LL);
			Thread.Sleep(10); //Give some time for it to apply
		}
		public static void StopHook() {
			if(CheckHook()) {
				return;
			}
			KMHook.UnhookWindowsHookEx(_hookID);
			KMHook.UnhookWindowsHookEx(_mouse_hookID);
			_hookID = _mouse_hookID = IntPtr.Zero;
			Thread.Sleep(10); //Give some time for it to apply
		}
		public static bool CheckHook() {
			return _hookID == IntPtr.Zero;
		}
		#endregion
	}
}
