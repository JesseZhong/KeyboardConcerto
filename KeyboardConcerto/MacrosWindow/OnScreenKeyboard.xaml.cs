// OnScreenKeyboard.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace KeyboardConcerto {

	using VKey = WindowsInput.VirtualKeyCode;

	public class OnScreenKeyboardClickEventArgs : EventArgs {
		public OnScreenKeyboardClickEventArgs (VKey key) : base() {
			this.Key = key;
		}

		public VKey Key {
			get;
			private set;
		}
	}

	public delegate void OnScreenKeyboardClick(object sender, OnScreenKeyboardClickEventArgs e);

	/// <summary>
	/// Interaction logic for OnScreenKeyboard.xaml
	/// </summary>
	public partial class OnScreenKeyboard : UserControl {

		#region Initialization
		/// <summary>
		/// Default constructor.
		/// </summary>
		public OnScreenKeyboard() {
			this.InitializeComponent();
		}
		#endregion

		#region Events
		/// <summary>
		/// 
		/// </summary>
		public OnScreenKeyboardClick ForwardedKey;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		private void ForwardVKey(VKey key) {
			if (this.ForwardedKey != null) {
				this.ForwardedKey(this, new OnScreenKeyboardClickEventArgs(key));
			}
		}
		#endregion

		private void KeyBrowserFavorites_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_FAVORITES);
		}

		private void KeyBrowserSearch_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_SEARCH);
		}

		private void KeyBrowserHome_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_HOME);
		}

		private void KeyBrowserBack_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_BACK);
		}

		private void KeyBrowserForward_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_FORWARD);
		}

		private void KeyBrowserRefresh_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_REFRESH);
		}

		private void KeyBrowserStop_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BROWSER_STOP);
		}

		private void KeyStop_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MEDIA_STOP);
		}

		private void KeyPrev_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MEDIA_PREV_TRACK);
		}

		private void KeyPlayPause_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MEDIA_PLAY_PAUSE);
		}

		private void KeyNext_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MEDIA_NEXT_TRACK);
		}

		private void KeyMute_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VOLUME_MUTE);
		}

		private void KeyVolumeDown_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VOLUME_DOWN);
		}

		private void KeyVolumeUp_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VOLUME_UP);
		}

		private void KeyEsc_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.ESCAPE);
		}

		private void KeyF1_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F1);
		}

		private void KeyF2_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F2);
		}

		private void KeyF3_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F3);
		}

		private void KeyF4_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F4);
		}

		private void KeyF5_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F5);
		}

		private void KeyF6_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F6);
		}

		private void KeyF7_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F7);
		}

		private void KeyF8_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F8);
		}

		private void KeyF9_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F9);
		}

		private void KeyF10_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F10);
		}

		private void KeyF11_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F11);
		}

		private void KeyF12_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.F12);
		}

		private void KeyPrtScrn_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.SNAPSHOT);
		}

		private void KeyScroll_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.SCROLL);
		}

		private void KeyPause_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.PAUSE);
		}

		private void KeyCalculator_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LAUNCH_APP1);
		}

		private void KeyPC_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LAUNCH_APP2);
		}

		private void KeyMail_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LAUNCH_MAIL);
		}

		private void KeyMedia_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LAUNCH_MEDIA_SELECT);
		}

		private void KeyGrave_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_3);
		}

		private void Key1_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_1);
		}

		private void Key2_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_2);
		}

		private void Key3_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_3);
		}

		private void Key4_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_4);
		}

		private void Key5_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_5);
		}

		private void Key6_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_6);
		}

		private void Key7_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_7);
		}

		private void Key8_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_8);
		}

		private void Key9_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_9);
		}

		private void Key0_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_0);
		}

		private void KeyHyphen_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_MINUS);
		}

		private void KeyEquals_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_PLUS);
		}

		private void KeyBackspace_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.BACK);
		}

		private void KeyTab_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.TAB);
		}

		private void KeyQ_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_Q);
		}

		private void KeyW_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_W);
		}

		private void KeyE_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_E);
		}

		private void KeyR_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_R);
		}

		private void KeyT_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_T);
		}

		private void KeyY_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_Y);
		}

		private void KeyU_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_U);
		}

		private void KeyI_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_I);
		}

		private void KeyO_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_O);
		}

		private void KeyP_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_P);
		}

		private void KeyLBracketClick(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_6);
		}

		private void KeyRBracket_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_4);
		}

		private void KeyBackSlash_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_5);
		}

		private void KeyCapsLock_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.CAPITAL);
		}

		private void KeyA_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_A);
		}

		private void KeyS_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_S);
		}

		private void KeyD_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_D);
		}

		private void KeyF_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_F);
		}

		private void KeyG_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_G);
		}

		private void KeyH_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_H);
		}

		private void KeyJ_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_J);
		}

		private void KeyK_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_K);
		}

		private void KeyL_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_L);
		}

		private void KeySemiColon_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_1);
		}

		private void KeyQuote_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_7);
		}

		private void KeyEnter_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RETURN);
		}

		private void KeyLShift_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LSHIFT);
		}

		private void KeyZ_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_Z);
		}

		private void KeyX_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_X);
		}

		private void KeyC_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_C);
		}

		private void KeyV_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_V);
		}

		private void KeyB_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_B);
		}

		private void KeyN_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_N);
		}

		private void KeyM_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.VK_M);
		}

		private void KeyComma_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_COMMA);
		}

		private void KeyPeriod_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_PERIOD);
		}

		private void KeySlash_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.OEM_2);
		}

		private void KeyRShift_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RSHIFT);
		}

		private void KeyLCtrl_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LCONTROL);
		}

		private void KeyLWin_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LWIN);
		}

		private void KeyLAlt_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LMENU);
		}

		private void KeySpace_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.SPACE);
		}

		private void KeyRAlt_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RMENU);
		}

		private void KeyRWin_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RWIN);
		}

		private void KeyMenu_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MENU);
		}

		private void KeyRCtrl_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RCONTROL);
		}

		private void KeyInsert_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.INSERT);
		}

		private void KeyHome_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.HOME);
		}

		private void KeyPageUp_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.PRIOR);
		}

		private void KeyDelete_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.DELETE);
		}

		private void KeyEnd_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.END);
		}

		private void KeyPageDown_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NEXT);
		}

		private void KeyUpArrow_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.UP);
		}

		private void KeyLeftArrow_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.LEFT);
		}

		private void KeyDownArrow_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.DOWN);
		}

		private void KeyRightArrow_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RIGHT);
		}

		private void KeyNumLock_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMLOCK);
		}

		private void KeyDivide_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.DIVIDE);
		}

		private void KeyMultiply_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.MULTIPLY);
		}

		private void KeyMinus_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.SUBTRACT);
		}

		private void KeyNum7_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD7);
		}

		private void KeyNum8_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD8);
		}

		private void KeyNum9_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD9);
		}

		private void KeyPlus_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.ADD);
		}

		private void KeyNum4_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD4);
		}

		private void KeyNum5_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD5);
		}

		private void KeyNum6_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD6);
		}

		private void KeyNum1_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD1);
		}

		private void KeyNum2_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD2);
		}

		private void KeyNum3_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD3);
		}

		private void KeyReturn_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.RETURN);
		}

		private void KeyNum0_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.NUMPAD0);
		}

		private void KeyNumDecimal_Click(object sender, RoutedEventArgs e) {
			this.ForwardVKey(VKey.DECIMAL);
		}
	}
}
