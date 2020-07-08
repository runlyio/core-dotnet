using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Runly.Client.Tests
{
	public class MemoryAccessTokenCacheTest
	{
		int tokenGenerationCount = 0;
		readonly MemoryAccessTokenCache cache = new MemoryAccessTokenCache();

		Func<Task<AccessToken>> GenerateTokenFunc(long expiresIn) => () =>
		{
			tokenGenerationCount++;
			return Task.FromResult(new AccessToken
			{
				TokenType = "Bearer",
				Value = "poopookachoo",
				ExpiresIn = expiresIn
			});
		};

		[Fact]
		public async Task Should_cache_access_token()
		{
			var result = await cache.GetToken("id", "secret", "me", GenerateTokenFunc(3600));

			result.Should().NotBeNull();
			result.ExpiresIn.Should().Be(3600);
			tokenGenerationCount.Should().Be(1);

			var result2 = await cache.GetToken("id", "secret", "me", GenerateTokenFunc(3600));

			result2.Should().Be(result);
			tokenGenerationCount.Should().Be(1);
		}

		[Fact]
		public async Task Should_not_cache_expired_access_token()
		{
			var result = await cache.GetToken("id", "secret", "me", GenerateTokenFunc(-100));

			result.Should().NotBeNull();
			result.ExpiresIn.Should().Be(-100);
			tokenGenerationCount.Should().Be(1);

			var result2 = await cache.GetToken("id", "secret", "me", GenerateTokenFunc(100));

			result2.Should().NotBe(result);
			result2.ExpiresIn.Should().Be(100);
			tokenGenerationCount.Should().Be(2);
		}

		[Theory]
		[InlineData("newid", "secret", "me")]
		[InlineData("id", "newsecret", "me")]
		[InlineData("id", "secret", "you")]
		public async Task Should_not_cache_access_token_for_different_client_or_audience(string clientId, string clientSecret, string audience)
		{
			var result = await cache.GetToken("id", "secret", "me", GenerateTokenFunc(3600));

			result.Should().NotBeNull();
			result.ExpiresIn.Should().Be(3600);
			tokenGenerationCount.Should().Be(1);

			var result2 = await cache.GetToken(clientId, clientSecret, audience, GenerateTokenFunc(3600));

			result2.Should().NotBe(result);
			result.ExpiresIn.Should().Be(3600);
			tokenGenerationCount.Should().Be(2);
		}
	}
}
