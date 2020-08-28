using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Runly
{
	public interface IAccessTokenCache
	{
		Task<AccessToken> GetToken(string clientId, string clientSecret, string audience, Func<Task<AccessToken>> tokenFactory);
	}

	public class MemoryAccessTokenCache : IAccessTokenCache
	{
		readonly ReaderWriterLockSlim lck = new ReaderWriterLockSlim();
		readonly Dictionary<string, Task<AccessToken>> tokens = new Dictionary<string, Task<AccessToken>>();

		public async Task<AccessToken> GetToken(string clientId, string clientSecret, string audience, Func<Task<AccessToken>> tokenFactory)
		{
			string key = $"{clientId}:{clientSecret}:{audience}";

			AccessToken token;
			Task<AccessToken> tokenTask;

			lck.EnterReadLock();
			try
			{
				tokens.TryGetValue(key, out tokenTask);
			}
			finally
			{
				lck.ExitReadLock();
			}

			if (tokenTask != null)
			{
				token = await tokenTask;
				if (token == null || token.IsExpired)
				{
					ClearToken(key);
				}
				else
				{
					return token;
				}
			}

			token = await AcquireToken(key, tokenFactory);
			return token;
		}

		Task<AccessToken> AcquireToken(string key, Func<Task<AccessToken>> tokenFactory)
		{
			lck.EnterWriteLock();
			try
			{
				if (tokens.ContainsKey(key))
					return tokens[key];

				var tokenTask = tokenFactory();
				tokens.Add(key, tokenTask);
				return tokenTask;
			}
			finally
			{
				lck.ExitWriteLock();
			}
		}

		bool ClearToken(string key)
		{
			lck.EnterWriteLock();
			try
			{
				return tokens.Remove(key);
			}
			finally
			{
				lck.ExitWriteLock();
			}
		}
	}
}
