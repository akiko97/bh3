using MoleMole.Config;

namespace MoleMole
{
	public class EmptyPageContext : BasePageContext
	{
		public EmptyPageContext()
		{
			config = new ContextPattern
			{
				contextName = "EmptyPageContext"
			};
		}
	}
}
