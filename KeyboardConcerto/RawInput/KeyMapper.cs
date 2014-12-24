using System.Globalization;
using System.Windows.Input;

namespace KeyboardConcerto.RawInput {
	public enum VirtualKeys : short {
		LeftButton = 0x01,
		RightButton = 0x02,
		Cancel = 0x03,
		MiddleButton = 0x04,
		ExtraButton1 = 0x05,
		ExtraButton2 = 0x06,
		Back = 0x08,
		Tab = 0x09,
		Clear = 0x0C,
		Return = 0x0D,
		Shift = 0x10,
		Control = 0x11,
		Menu = 0x12,
		Pause = 0x13,
		CapsLock = 0x14,
		Kana = 0x15,
		Hangeul = 0x15,
		Hangul = 0x15,
		Junja = 0x17,
		Final = 0x18,
		Hanja = 0x19,
		Kanji = 0x19,
		Escape = 0x1B,
		Convert = 0x1C,
		NonConvert = 0x1D,
		Accept = 0x1E,
		ModeChange = 0x1F,
		Space = 0x20,
		Prior = 0x21,
		Next = 0x22,
		End = 0x23,
		Home = 0x24,
		Left = 0x25,
		Up = 0x26,
		Right = 0x27,
		Down = 0x28,
		Select = 0x29,
		Print = 0x2A,
		Execute = 0x2B,
		Snapshot = 0x2C,
		Insert = 0x2D,
		Delete = 0x2E,
		Help = 0x2F,
		D0 = 0x30,
		D1 = 0x31,
		D2 = 0x32,
		D3 = 0x33,
		D4 = 0x34,
		D5 = 0x35,
		D6 = 0x36,
		D7 = 0x37,
		D8 = 0x38,
		D9 = 0x39,
		A = 0x41,
		B = 0x42,
		C = 0x43,
		D = 0x44,
		E = 0x45,
		F = 0x46,
		G = 0x47,
		H = 0x48,
		I = 0x49,
		J = 0x4A,
		K = 0x4B,
		L = 0x4C,
		M = 0x4D,
		N = 0x4E,
		O = 0x4F,
		P = 0x50,
		Q = 0x51,
		R = 0x52,
		S = 0x53,
		T = 0x54,
		U = 0x55,
		V = 0x56,
		W = 0x57,
		X = 0x58,
		Y = 0x59,
		Z = 0x5A,
		LeftWindows = 0x5B,
		RightWindows = 0x5C,
		Application = 0x5D,
		Sleep = 0x5F,
		NumPad0 = 0x60,
		NumPad1 = 0x61,
		NumPad2 = 0x62,
		NumPad3 = 0x63,
		NumPad4 = 0x64,
		NumPad5 = 0x65,
		NumPad6 = 0x66,
		NumPad7 = 0x67,
		NumPad8 = 0x68,
		NumPad9 = 0x69,
		Multiply = 0x6A,
		Add = 0x6B,
		Separator = 0x6C,
		Subtract = 0x6D,
		Decimal = 0x6E,
		Divide = 0x6F,
		F1 = 0x70,
		F2 = 0x71,
		F3 = 0x72,
		F4 = 0x73,
		F5 = 0x74,
		F6 = 0x75,
		F7 = 0x76,
		F8 = 0x77,
		F9 = 0x78,
		F10 = 0x79,
		F11 = 0x7A,
		F12 = 0x7B,
		F13 = 0x7C,
		F14 = 0x7D,
		F15 = 0x7E,
		F16 = 0x7F,
		F17 = 0x80,
		F18 = 0x81,
		F19 = 0x82,
		F20 = 0x83,
		F21 = 0x84,
		F22 = 0x85,
		F23 = 0x86,
		F24 = 0x87,
		NumLock = 0x90,
		ScrollLock = 0x91,
		NEC_Equal = 0x92,
		Fujitsu_Jisho = 0x92,
		Fujitsu_Masshou = 0x93,
		Fujitsu_Touroku = 0x94,
		Fujitsu_Loya = 0x95,
		Fujitsu_Roya = 0x96,
		LeftShift = 0xA0,
		RightShift = 0xA1,
		LeftControl = 0xA2,
		RightControl = 0xA3,
		LeftMenu = 0xA4,
		RightMenu = 0xA5,
		BrowserBack = 0xA6,
		BrowserForward = 0xA7,
		BrowserRefresh = 0xA8,
		BrowserStop = 0xA9,
		BrowserSearch = 0xAA,
		BrowserFavorites = 0xAB,
		BrowserHome = 0xAC,
		VolumeMute = 0xAD,
		VolumeDown = 0xAE,
		VolumeUp = 0xAF,
		MediaNextTrack = 0xB0,
		MediaPrevTrack = 0xB1,
		MediaStop = 0xB2,
		MediaPlayPause = 0xB3,
		LaunchMail = 0xB4,
		LaunchMediaSelect = 0xB5,
		LaunchApplication1 = 0xB6,
		LaunchApplication2 = 0xB7,
		OEM1 = 0xBA,
		OEMPlus = 0xBB,
		OEMComma = 0xBC,
		OEMMinus = 0xBD,
		OEMPeriod = 0xBE,
		OEM2 = 0xBF,
		OEM3 = 0xC0,
		OEM4 = 0xDB,
		OEM5 = 0xDC,
		OEM6 = 0xDD,
		OEM7 = 0xDE,
		OEM8 = 0xDF,
		OEMAX = 0xE1,
		OEM102 = 0xE2,
		ICOHelp = 0xE3,
		ICO00 = 0xE4,
		ProcessKey = 0xE5,
		ICOClear = 0xE6,
		Packet = 0xE7,
		OEMReset = 0xE9,
		OEMJump = 0xEA,
		OEMPA1 = 0xEB,
		OEMPA2 = 0xEC,
		OEMPA3 = 0xED,
		OEMWSCtrl = 0xEE,
		OEMCUSel = 0xEF,
		OEMATTN = 0xF0,
		OEMFinish = 0xF1,
		OEMCopy = 0xF2,
		OEMAuto = 0xF3,
		OEMENLW = 0xF4,
		OEMBackTab = 0xF5,
		ATTN = 0xF6,
		CRSel = 0xF7,
		EXSel = 0xF8,
		EREOF = 0xF9,
		Play = 0xFA,
		Zoom = 0xFB,
		Noname = 0xFC,
		PA1 = 0xFD,
		OEMClear = 0xFE
	}

	public static class KeyMapper {
		// I prefer to have control over the key mapping
		// This mapping could be loading from file to allow mapping changes without a recompile
		public static string GetKeyName(int value) {
			switch (value) {
				case 0x41: return "A";
				case 0x6b: return "Add";
				case 0x40000: return "Alt";
				case 0x5d: return "Apps";
				case 0xf6: return "Attn";
				case 0x42: return "B";
				case 8: return "Back";
				case 0xa6: return "BrowserBack";
				case 0xab: return "BrowserFavorites";
				case 0xa7: return "BrowserForward";
				case 0xac: return "BrowserHome";
				case 0xa8: return "BrowserRefresh";
				case 170: return "BrowserSearch";
				case 0xa9: return "BrowserStop";
				case 0x43: return "C";
				case 3: return "Cancel";
				case 20: return "Capital";
				//case 20:      return "CapsLock";
				case 12: return "Clear";
				case 0x20000: return "Control";
				case 0x11: return "ControlKey";
				case 0xf7: return "Crsel";
				case 0x44: return "D";
				case 0x30: return "D0";
				case 0x31: return "D1";
				case 50: return "D2";
				case 0x33: return "D3";
				case 0x34: return "D4";
				case 0x35: return "D5";
				case 0x36: return "D6";
				case 0x37: return "D7";
				case 0x38: return "D8";
				case 0x39: return "D9";
				case 110: return "Decimal";
				case 0x2e: return "Delete";
				case 0x6f: return "Divide";
				case 40: return "Down";
				case 0x45: return "E";
				case 0x23: return "End";
				case 13: return "Enter";
				case 0xf9: return "EraseEof";
				case 0x1b: return "Escape";
				case 0x2b: return "Execute";
				case 0xf8: return "Exsel";
				case 70: return "F";
				case 0x70: return "F1";
				case 0x79: return "F10";
				case 0x7a: return "F11";
				case 0x7b: return "F12";
				case 0x7c: return "F13";
				case 0x7d: return "F14";
				case 0x7e: return "F15";
				case 0x7f: return "F16";
				case 0x80: return "F17";
				case 0x81: return "F18";
				case 130: return "F19";
				case 0x71: return "F2";
				case 0x83: return "F20";
				case 0x84: return "F21";
				case 0x85: return "F22";
				case 0x86: return "F23";
				case 0x87: return "F24";
				case 0x72: return "F3";
				case 0x73: return "F4";
				case 0x74: return "F5";
				case 0x75: return "F6";
				case 0x76: return "F7";
				case 0x77: return "F8";
				case 120: return "F9";
				case 0x18: return "FinalMode";
				case 0x47: return "G";
				case 0x48: return "H";
				case 0x15: return "HanguelMode";
				//case 0x15:    return "HangulMode";
				case 0x19: return "HanjaMode";
				case 0x2f: return "Help";
				case 0x24: return "Home";
				case 0x49: return "I";
				case 30: return "IMEAceept";
				case 0x1c: return "IMEConvert";
				case 0x1f: return "IMEModeChange";
				case 0x1d: return "IMENonconvert";
				case 0x2d: return "Insert";
				case 0x4a: return "J";
				case 0x17: return "JunjaMode";
				case 0x4b: return "K";
				//case 0x15:    return "KanaMode";
				//case 0x19:    return "KanjiMode";
				case 0xffff: return "KeyCode";
				case 0x4c: return "L";
				case 0xb6: return "LaunchApplication1";
				case 0xb7: return "LaunchApplication2";
				case 180: return "LaunchMail";
				case 1: return "LButton";
				case 0xa2: return "LControl";
				case 0x25: return "Left";
				case 10: return "LineFeed";
				case 0xa4: return "LMenu";
				case 160: return "LShift";
				case 0x5b: return "LWin";
				case 0x4d: return "M";
				case 4: return "MButton";
				case 0xb0: return "MediaNextTrack";
				case 0xb3: return "MediaPlayPause";
				case 0xb1: return "MediaPreviousTrack";
				case 0xb2: return "MediaStop";
				case 0x12: return "Menu";
				// case 65536:  return "Modifiers";
				case 0x6a: return "Multiply";
				case 0x4e: return "N";
				case 0x22: return "Next";
				case 0xfc: return "NoName";
				case 0: return "None";
				case 0x90: return "NumLock";
				case 0x60: return "NumPad0";
				case 0x61: return "NumPad1";
				case 0x62: return "NumPad2";
				case 0x63: return "NumPad3";
				case 100: return "NumPad4";
				case 0x65: return "NumPad5";
				case 0x66: return "NumPad6";
				case 0x67: return "NumPad7";
				case 0x68: return "NumPad8";
				case 0x69: return "NumPad9";
				case 0x4f: return "O";
				case 0xdf: return "Oem8";
				case 0xe2: return "OemBackslash";
				case 0xfe: return "OemClear";
				case 0xdd: return "OemCloseBrackets";
				case 0xbc: return "OemComma";
				case 0xbd: return "OemMinus";
				case 0xdb: return "OemOpenBrackets";
				case 190: return "OemPeriod";
				case 220: return "OemPipe";
				case 0xbb: return "Oemplus";
				case 0xbf: return "OemQuestion";
				case 0xde: return "OemQuotes";
				case 0xba: return "OemSemicolon";
				case 0xc0: return "Oemtilde";
				case 80: return "P";
				case 0xfd: return "Pa1";
				// case 0x22:   return "PageDown";
				// case 0x21:   return "PageUp";
				case 0x13: return "Pause";
				case 250: return "Play";
				case 0x2a: return "Print";
				case 0x2c: return "PrintScreen";
				case 0x21: return "Prior";
				case 0xe5: return "ProcessKey";
				case 0x51: return "Q";
				case 0x52: return "R";
				case 2: return "RButton";
				case 0xa3: return "RControl";
				//case 13:      return "Return";
				case 0x27: return "Right";
				case 0xa5: return "RMenu";
				case 0xa1: return "RShift";
				case 0x5c: return "RWin";
				case 0x53: return "S";
				case 0x91: return "Scroll";
				case 0x29: return "Select";
				case 0xb5: return "SelectMedia";
				case 0x6c: return "Separator";
				case 0x10000: return "Shift";
				case 0x10: return "ShiftKey";
				//case 0x2c:    return "Snapshot";
				case 0x20: return "Space";
				case 0x6d: return "Subtract";
				case 0x54: return "T";
				case 9: return "Tab";
				case 0x55: return "U";
				case 0x26: return "Up";
				case 0x56: return "V";
				case 0xae: return "VolumeDown";
				case 0xad: return "VolumeMute";
				case 0xaf: return "VolumeUp";
				case 0x57: return "W";
				case 0x58: return "X";
				case 5: return "XButton1";
				case 6: return "XButton2";
				case 0x59: return "Y";
				case 90: return "Z";
				case 0xfb: return "Zoom";
			}

			return value.ToString(CultureInfo.InvariantCulture).ToUpper();
		}

		// If you prefer the virtualkey converted into a Microsoft virtualkey code use this
		public static string GetMicrosoftKeyName(int virtualKey) {
			return new KeyConverter().ConvertToString(virtualKey);
		}
	}
}
