using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public abstract class BaseAvatarAction : Action
	{
		protected BaseMonoAvatar _avatar;

		protected AvatarActor _avatarActor;

		protected AvatarAIPlugin _avatarAIPlugin;

		public override void OnAwake()
		{
			_avatar = GetComponent<BaseMonoAvatar>();
			_avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(_avatar.GetRuntimeID());
			_avatarAIPlugin = _avatarActor.GetPlugin<AvatarAIPlugin>();
		}
	}
}
