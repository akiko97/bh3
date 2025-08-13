using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace MoleMole
{
	public class MiHoYoGameDataV2
	{
		public const string PLAYERPREFS_KEY = "MGD_V2";

		private const byte XOR_CODE_MEMORY = 169;

		private const byte XOR_CODE_DISK = 104;

		private static MiHoYoGameDataV2 _instance;

		private byte[] _dataBytes;

		private int _dataLength;

		private byte[] _indexBytes;

		private int _indexLength;

		private List<string> _keyIndex;

		private List<int> _posIndex;

		private List<int> _lenIndex;

		private MiHoYoGameDataV2(bool independent)
		{
			if (!independent)
			{
				if (!PlayerPrefs.HasKey("MGD_V2"))
				{
					ResetData();
				}
				else
				{
					Deserialize();
				}
			}
			else
			{
				ResetStructure();
			}
		}

		public static MiHoYoGameDataV2 GetInstance()
		{
			if (_instance == null)
			{
				_instance = new MiHoYoGameDataV2(false);
			}
			if (_instance == null)
			{
				throw new Exception("MGD Singleton Null Error");
			}
			return _instance;
		}

		public static MiHoYoGameDataV2 CreateIndepedentOne()
		{
			return new MiHoYoGameDataV2(true);
		}

		public bool HasKey(string key)
		{
			CheckThreadSafe();
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveKey(string key)
		{
			CheckThreadSafe();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				int num2 = _posIndex[num];
				int num3 = _lenIndex[num];
				_keyIndex.RemoveAt(num);
				for (int j = num + 1; j < _posIndex.Count; j++)
				{
					List<int> posIndex;
					List<int> list = (posIndex = _posIndex);
					int index2;
					int index = (index2 = j);
					index2 = posIndex[index2];
					list[index] = index2 - num3;
				}
				_posIndex.RemoveAt(num);
				_lenIndex.RemoveAt(num);
				int num4 = _dataLength - num3;
				byte[] array = new byte[num4];
				Array.Copy(_dataBytes, 0, array, 0, num2);
				Array.Copy(_dataBytes, num2 + num3, array, num2, _dataLength - num2 - num3);
				_dataBytes = array;
				_dataLength = num4;
			}
		}

		public int GetInt(string key)
		{
			CheckThreadSafe();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				byte[] array = new byte[Marshal.SizeOf(typeof(int))];
				Array.Copy(_dataBytes, _posIndex[num], array, 0, _lenIndex[num]);
				XORByteArrayMemory(array);
				return BitConverter.ToInt32(array, 0);
			}
			return 0;
		}

		public void SetInt(string key, int value)
		{
			CheckThreadSafe();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			byte[] bytes;
			if (num >= 0)
			{
				bytes = BitConverter.GetBytes(value);
				XORByteArrayMemory(bytes);
				bytes.CopyTo(_dataBytes, _posIndex[num]);
				return;
			}
			_keyIndex.Add(key);
			_posIndex.Add(_dataLength);
			_lenIndex.Add(Marshal.SizeOf(typeof(int)));
			int num2 = _dataLength + Marshal.SizeOf(typeof(int));
			byte[] array = new byte[num2];
			Array.Copy(_dataBytes, 0, array, 0, _dataLength);
			bytes = BitConverter.GetBytes(value);
			XORByteArrayMemory(bytes);
			bytes.CopyTo(array, _dataLength);
			_dataLength = num2;
			_dataBytes = array;
		}

		public float GetFloat(string key)
		{
			CheckThreadSafe();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				byte[] array = new byte[Marshal.SizeOf(typeof(float))];
				Array.Copy(_dataBytes, _posIndex[num], array, 0, _lenIndex[num]);
				XORByteArrayMemory(array);
				return BitConverter.ToSingle(array, 0);
			}
			return 0f;
		}

		public void SetFloat(string key, float value)
		{
			CheckThreadSafe();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			byte[] bytes;
			if (num >= 0)
			{
				bytes = BitConverter.GetBytes(value);
				XORByteArrayMemory(bytes);
				bytes.CopyTo(_dataBytes, _posIndex[num]);
				return;
			}
			_keyIndex.Add(key);
			_posIndex.Add(_dataLength);
			_lenIndex.Add(Marshal.SizeOf(typeof(float)));
			int num2 = _dataLength + Marshal.SizeOf(typeof(float));
			byte[] array = new byte[num2];
			Array.Copy(_dataBytes, 0, array, 0, _dataLength);
			bytes = BitConverter.GetBytes(value);
			XORByteArrayMemory(bytes);
			bytes.CopyTo(array, _dataLength);
			_dataLength = num2;
			_dataBytes = array;
		}

		public string GetString(string key)
		{
			CheckThreadSafe();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				byte[] array = new byte[_lenIndex[num]];
				Array.Copy(_dataBytes, _posIndex[num], array, 0, _lenIndex[num]);
				XORByteArrayMemory(array);
				return uTF8Encoding.GetString(array);
			}
			return string.Empty;
		}

		public void SetString(string key, string value)
		{
			CheckThreadSafe();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			int num = -1;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				if (_keyIndex[i] == key)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				byte[] bytes = uTF8Encoding.GetBytes(value);
				XORByteArrayMemory(bytes);
				int num2 = bytes.Length;
				int num3 = num2 - _lenIndex[num];
				for (int j = num + 1; j < _posIndex.Count; j++)
				{
					List<int> posIndex;
					List<int> list = (posIndex = _posIndex);
					int index2;
					int index = (index2 = j);
					index2 = posIndex[index2];
					list[index] = index2 + num3;
				}
				int num4 = _dataLength + num3;
				byte[] array = new byte[num4];
				Array.Copy(_dataBytes, 0, array, 0, _posIndex[num]);
				Array.Copy(bytes, 0, array, _posIndex[num], num2);
				Array.Copy(_dataBytes, _posIndex[num] + _lenIndex[num], array, _posIndex[num] + num2, _dataLength - _posIndex[num] - _lenIndex[num]);
				_lenIndex[num] = num2;
				_dataLength = num4;
				_dataBytes = array;
			}
			else
			{
				_keyIndex.Add(key);
				_posIndex.Add(_dataLength);
				byte[] bytes = uTF8Encoding.GetBytes(value);
				XORByteArrayMemory(bytes);
				int num2 = bytes.Length;
				_lenIndex.Add(num2);
				int num4 = _dataLength + num2;
				byte[] array = new byte[num4];
				Array.Copy(_dataBytes, 0, array, 0, _dataLength);
				Array.Copy(bytes, 0, array, _dataLength, num2);
				_dataLength = num4;
				_dataBytes = array;
			}
		}

		public string SerializeToString()
		{
			CheckThreadSafe();
			SerializeIndex();
			int num = Marshal.SizeOf(_indexLength) + _indexBytes.Length + Marshal.SizeOf(_dataLength) + _dataBytes.Length;
			byte[] array = new byte[num];
			int num2 = 0;
			BitConverter.GetBytes(_indexLength).CopyTo(array, num2);
			num2 += Marshal.SizeOf(_indexLength);
			Array.Copy(_indexBytes, 0, array, num2, _indexLength);
			num2 += _indexBytes.Length;
			BitConverter.GetBytes(_dataLength).CopyTo(array, num2);
			num2 += Marshal.SizeOf(_dataLength);
			Array.Copy(_dataBytes, 0, array, num2, _dataLength);
			num2 += _dataBytes.Length;
			XORByteArrayDisk(array);
			string text = Convert.ToBase64String(array);
			string text2 = SecurityUtil.Md5(text + GetMD5MagicCode());
			return text2 + text;
		}

		public void Serialize()
		{
			CheckThreadSafe();
			PlayerPrefs.SetString("MGD_V2", SerializeToString());
			PlayerPrefs.Save();
		}

		public void DeserializeFromString(string fullDataStr)
		{
			CheckThreadSafe();
			string text = fullDataStr.Substring(32, fullDataStr.Length - 32);
			string text2 = fullDataStr.Substring(0, 32);
			string text3 = SecurityUtil.Md5(text + GetMD5MagicCode());
			if (text3 != text2)
			{
				PlayerPrefs.DeleteKey("MGD_V2");
				ResetData();
				return;
			}
			byte[] array = Convert.FromBase64String(text);
			XORByteArrayDisk(array);
			int num = 0;
			_indexLength = BitConverter.ToInt32(array, num);
			num += Marshal.SizeOf(_indexLength);
			_indexBytes = new byte[_indexLength];
			Array.Copy(array, num, _indexBytes, 0, _indexLength);
			num += _indexLength;
			_dataLength = BitConverter.ToInt32(array, num);
			num += Marshal.SizeOf(_dataLength);
			_dataBytes = new byte[_dataLength];
			Array.Copy(array, num, _dataBytes, 0, _dataLength);
			num += _dataLength;
			DeserializeIndex();
		}

		private void Deserialize()
		{
			CheckThreadSafe();
			string fullDataStr = PlayerPrefs.GetString("MGD_V2");
			DeserializeFromString(fullDataStr);
		}

		private void SerializeIndex()
		{
			CheckThreadSafe();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			List<byte[]> list = new List<byte[]>();
			int num = 0;
			for (int i = 0; i < _keyIndex.Count; i++)
			{
				list.Add(uTF8Encoding.GetBytes(_keyIndex[i]));
				num += list[i].Length;
			}
			_indexLength = Marshal.SizeOf(typeof(int)) * _keyIndex.Count + Marshal.SizeOf(typeof(int)) * _posIndex.Count + Marshal.SizeOf(typeof(int)) * _lenIndex.Count + num + Marshal.SizeOf(typeof(int));
			_indexBytes = new byte[_indexLength];
			int num2 = 0;
			BitConverter.GetBytes(_keyIndex.Count).CopyTo(_indexBytes, num2);
			num2 += Marshal.SizeOf(_keyIndex.Count);
			for (int j = 0; j < _keyIndex.Count; j++)
			{
				BitConverter.GetBytes(list[j].Length).CopyTo(_indexBytes, num2);
				num2 += Marshal.SizeOf(list[j].Length);
				Array.Copy(list[j], 0, _indexBytes, num2, list[j].Length);
				num2 += list[j].Length;
				BitConverter.GetBytes(_posIndex[j]).CopyTo(_indexBytes, num2);
				num2 += Marshal.SizeOf(_posIndex[j]);
				BitConverter.GetBytes(_lenIndex[j]).CopyTo(_indexBytes, num2);
				num2 += Marshal.SizeOf(_lenIndex[j]);
			}
		}

		private void DeserializeIndex()
		{
			CheckThreadSafe();
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			int num = 0;
			int num2 = BitConverter.ToInt32(_indexBytes, num);
			num += Marshal.SizeOf(num2);
			_keyIndex = new List<string>();
			_posIndex = new List<int>();
			_lenIndex = new List<int>();
			for (int i = 0; i < num2; i++)
			{
				int num3 = BitConverter.ToInt32(_indexBytes, num);
				num += Marshal.SizeOf(num3);
				byte[] array = new byte[num3];
				Array.Copy(_indexBytes, num, array, 0, num3);
				num += array.Length;
				_keyIndex.Add(uTF8Encoding.GetString(array));
				_posIndex.Add(BitConverter.ToInt32(_indexBytes, num));
				num += Marshal.SizeOf(_posIndex[i]);
				_lenIndex.Add(BitConverter.ToInt32(_indexBytes, num));
				num += Marshal.SizeOf(_lenIndex[i]);
			}
		}

		private void ResetStructure()
		{
			_keyIndex = new List<string>();
			_posIndex = new List<int>();
			_lenIndex = new List<int>();
			_dataLength = 0;
			_dataBytes = new byte[0];
		}

		public void ResetData()
		{
			CheckThreadSafe();
			ResetStructure();
			Serialize();
			Deserialize();
		}

		private string GetMD5MagicCode()
		{
			return "p]e!IO6SoR~d-BcBb^dTkVcx2015";
		}

		private void CheckThreadSafe()
		{
			PlayerPrefs.HasKey("TestThread");
		}

		private void XORByteArrayMemory(byte[] byteArray)
		{
			for (int i = 0; i < byteArray.Length; i++)
			{
				byteArray[i] ^= 169;
			}
		}

		private void XORByteArrayDisk(byte[] byteArray)
		{
			for (int i = 0; i < byteArray.Length; i++)
			{
				byteArray[i] ^= 104;
			}
		}
	}
}
