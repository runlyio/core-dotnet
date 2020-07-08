using System;
using Newtonsoft.Json;

namespace Runly
{
	public class AccessToken
	{
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		[JsonProperty("access_token")]
		public string Value { get; set; }

		long expiresIn;
		[JsonProperty("expires_in")]
		public long ExpiresIn
		{
			get { return expiresIn; }
			set
			{
				expiresIn = value;

				// mark it expired 10 seconds before it really expires
				ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 10);
			}
		}

		public DateTime ExpiresAt { get; private set; }

		public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
	}
}
