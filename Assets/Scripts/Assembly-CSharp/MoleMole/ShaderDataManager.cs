using System;

namespace MoleMole
{
	public class ShaderDataManager
	{
		private MonoBuffShader_Base[] ShaderDataList = new MonoBuffShader_Base[Enum.GetValues(typeof(E_ShaderData)).Length];

		private ShaderDataManager()
		{
		}

		public void InitAtAwake()
		{
			E_ShaderData[] array = new E_ShaderData[2]
			{
				E_ShaderData.ColorBias,
				E_ShaderData.AvatarHelper
			};
			E_ShaderData[] array2 = array;
			foreach (E_ShaderData e_ShaderData in array2)
			{
				ShaderDataList[(int)e_ShaderData] = Singleton<AuxObjectManager>.Instance.LoadAuxObjectProto(e_ShaderData.ToString()).GetComponent<MonoBuffShader_Base>();
			}
		}

		public T GetBuffShaderData<T>(E_ShaderData buff) where T : MonoBuffShader_Base
		{
			if (ShaderDataList[(int)buff] == null)
			{
				ShaderDataList[(int)buff] = Singleton<AuxObjectManager>.Instance.LoadAuxObjectProto(buff.ToString()).GetComponent<MonoBuffShader_Base>();
			}
			return (T)ShaderDataList[(int)buff];
		}
	}
}
