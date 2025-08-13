namespace RenderHeads.Media.AVProVideo
{
	public interface IMediaInfo
	{
		float GetDurationMs();

		int GetVideoWidth();

		int GetVideoHeight();

		float GetVideoFrameRate();

		float GetVideoDisplayRate();

		bool HasVideo();

		bool HasAudio();

		int GetAudioTrackCount();
	}
}
