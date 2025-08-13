using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoleMole
{
	public class SecurityUtil
	{
		public static void RSAEncrypted(ref byte[] bytes, string rsaPublic)
		{
			try
			{
				using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
				{
					rSACryptoServiceProvider.FromXmlString(rsaPublic);
					bytes = rSACryptoServiceProvider.Encrypt(bytes, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public static void RSADecrypted(ref byte[] bytes, string rsaPrivate)
		{
			try
			{
				using (RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider())
				{
					rSACryptoServiceProvider.FromXmlString(rsaPrivate);
					bytes = rSACryptoServiceProvider.Decrypt(bytes, false);
				}
			}
			catch (Exception)
			{
			}
		}

		public static void AESEncrypted(ref byte[] bytes, byte[] key, byte[] iv)
		{
			try
			{
				using (Rijndael rijndael = Rijndael.Create())
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(key, iv), CryptoStreamMode.Write))
						{
							cryptoStream.Write(bytes, 0, bytes.Length);
							cryptoStream.FlushFinalBlock();
							bytes = memoryStream.ToArray();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static void AESDecrypted(ref byte[] bytes, byte[] key, byte[] iv)
		{
			try
			{
				using (Rijndael rijndael = Rijndael.Create())
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(key, iv), CryptoStreamMode.Write))
						{
							cryptoStream.Write(bytes, 0, bytes.Length);
							cryptoStream.FlushFinalBlock();
							bytes = memoryStream.ToArray();
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static string CalculateFileHash(byte[] fileBytes, byte[] hmacKey)
		{
			try
			{
				string text = string.Empty;
				using (HMACSHA1 hMACSHA = new HMACSHA1(hmacKey))
				{
					byte[] array = hMACSHA.ComputeHash(fileBytes);
					for (int i = 0; i < array.Length; i++)
					{
						text += array[i].ToString("X").PadLeft(2, '0');
					}
				}
				return text;
			}
			catch (Exception)
			{
			}
			return string.Empty;
		}

		public static string Md5(string strToEncrypt)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
			return Md5(bytes);
		}

		public static string Md5(byte[] bytes)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}

		public static string SHA1(string strToEncrypt)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
			SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			byte[] array = sHA1CryptoServiceProvider.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}

		public static string Base64Encoder(string strToEncode)
		{
			string empty = string.Empty;
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			try
			{
				return Convert.ToBase64String(uTF8Encoding.GetBytes(strToEncode));
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string Base64Decoder(string strToDecode)
		{
			string empty = string.Empty;
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			try
			{
				return uTF8Encoding.GetString(Convert.FromBase64String(strToDecode));
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string SHA256(string strToEncrypt)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
			SHA256Managed sHA256Managed = new SHA256Managed();
			byte[] array = sHA256Managed.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}

		public static string SHA256(byte[] bytes)
		{
			SHA256Managed sHA256Managed = new SHA256Managed();
			byte[] array = sHA256Managed.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}
	}
}
