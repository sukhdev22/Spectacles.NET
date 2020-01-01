using System;
using Newtonsoft.Json;

namespace Spectacles.NET.Rest.APIError
{
	/// <summary>
	/// JSON Discord Api Error Response
	/// </summary>
	public class DiscordAPIErrorResponse
	{
		/// <summary>
		/// Status Code of JSON Discord Api Error Response
		/// </summary>
		[JsonProperty("code")]
		public int Code { get; set; }

		/// <summary>
		/// Message of JSON Discord Api Error Response
		/// </summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}

	/// <inheritdoc />
	/// <summary>
	///     Class representing a Discord API exception.
	/// </summary>
	public class DiscordAPIException : Exception
	{
		/// <inheritdoc />
		/// <summary>
		///     Creates a new Instance of DiscordAPIException
		/// </summary>
		/// <param name="statusCode">The StatusCode of the Exception.</param>
		/// <param name="errorCode">Optional ErrorCode of the Exception.</param>
		/// <param name="errorMessages">Optional ErrorMessage of the Exception.</param>
		public DiscordAPIException(int statusCode, int? errorCode, string errorMessages) : base(
			$"{errorCode ?? statusCode}: {errorMessages}")
		{
			StatusCode = statusCode;
			ErrorCode = errorCode;
			ErrorMessages = errorMessages;
		}

		/// <summary>
		///     The StatusCode of the Exception.
		/// </summary>
		public int StatusCode { get; }

		/// <summary>
		///     Optional ErrorCode of the Exception.
		/// </summary>
		public int? ErrorCode { get; }

		/// <summary>
		///     Optional ErrorMessage of the Exception.
		/// </summary>
		public string ErrorMessages { get; }
	}
}
