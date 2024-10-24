namespace Knowit.Umbraco.TokenReplacement.Service
{
	public interface ICultureExtractor
	{
		string? GetCultureFromUrl(string host, string path);
	}
}
