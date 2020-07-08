using System;
using FluentAssertions;
using Xunit;

namespace Runly.Client.Tests
{
	public class AccessTokenTest
	{
		[Fact]
		public void Should_calculate_expiration_date_for_token_that_expires_in_future()
		{
			var token = new AccessToken
			{
				TokenType = "Bearer",
				Value = "poopookachoo",
				ExpiresIn = 3600
			};

			token.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddSeconds(3590));
			token.IsExpired.Should().BeFalse();
		}

		[Fact]
		public void Should_calculate_expiration_date_for_expired_token()
		{
			var token = new AccessToken
			{
				TokenType = "Bearer",
				Value = "poopookachoo",
				ExpiresIn = 1
			};

			token.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddSeconds(-9));
			token.IsExpired.Should().BeTrue();
		}
	}
}
