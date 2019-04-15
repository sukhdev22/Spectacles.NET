using System.Net.Http;
using System.Threading.Tasks;

namespace Spectacles.NET.Rest.Bucket
{
	/// <summary>
	/// Bucket for handling Ratelimits of one Route.
	/// </summary>
	public interface IBucket
	{
		/// <summary>
		/// The RestClient which instantiated this Bucket.
		/// </summary>
		RestClient Client { get; }

		/// <summary>
		/// Enqueues a Request in this Bucket.
		/// </summary>
		/// <param name="method">The HTTP Request method to use.</param>
		/// <param name="url">The URL to use.</param>
		/// <param name="content">The HTTPContent to use.</param>
		/// <param name="reason">Optional AuditLog reason to use.</param>
		/// <returns>Task resolving with response from Discord API</returns>
		Task<object> Enqueue(RequestMethod method, string url, HttpContent content, string reason);

		/// <summary>
		/// Enqueues a Request in this Bucket.
		/// </summary>
		/// <param name="method">The HTTP Request method to use.</param>
		/// <param name="url">The URL to use.</param>
		/// <param name="content">The HTTPContent to use.</param>
		/// <param name="reason">Optional AuditLog reason to use.</param>
		/// <returns>Task resolving with response from Discord API</returns>
		Task<T> Enqueue<T>(RequestMethod method, string url, HttpContent content, string reason);

		/// <summary>
		/// Enqueues a Request in this Bucket.
		/// </summary>
		/// <param name="request">The request to enqueue</param>
		void Enqueue(Request request);

		/// <summary>
		/// Sets the amount of request that can be done before waiting.
		/// </summary>
		Task SetLimit(int amount);
		
		/// <summary>
		/// Sets the remaining amount of request that can be done. 
		/// </summary>
		Task SetRemaining(int amount);
		
		/// <summary>
		/// Sets the timeout to wait before continue sending requests.
		/// </summary>
		Task SetTimeout(int amount);

		/// <summary>
		/// Gets the amount of request that can be done before waiting.
		/// </summary>
		Task<int> GetLimit();
		
		/// <summary>
		/// Gets the remaining amount of request that can be done. 
		/// </summary>
		Task<int> GetRemaining();
		
		/// <summary>
		/// Gets the timeout to wait before continue sending requests.
		/// </summary>
		Task<int> GetTimeout();

		/// <summary>
		/// If this Bucket is currently Ratelimited or if this Bot is Globally Ratelimited
		/// </summary>
		Task<bool> IsLimited();

		/// <summary>
		/// If the bot is Globally Ratelimited
		/// </summary>
		Task<bool> IsGloballyLimited();

		/// <summary>
		/// Sets The IsGloballyLimited property
		/// </summary>
		/// <returns></returns>
		Task SetGloballyLimited(int until);
	}
}