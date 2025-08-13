namespace MoleMole
{
	public sealed class MonoKiana_C2 : MonoKiana
	{
		protected override void PostInit()
		{
			base.PostInit();
			_transform.Find("Avatar_Kiana_C2_Rope_Left_1").parent = null;
			_transform.Find("Avatar_Kiana_C2_Rope_Left_2").parent = null;
			_transform.Find("Avatar_Kiana_C2_Rope_Right_1").parent = null;
			_transform.Find("Avatar_Kiana_C2_Rope_Right_2").parent = null;
		}
	}
}
