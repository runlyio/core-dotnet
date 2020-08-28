using Runly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Runly
{
	public interface IOrgClient
	{
		Task<IEnumerable<Plan>> GetPlans(string org = null);

		Task<Organization> GetOrg(string org);
		Task<Organization> CreateOrganization(NewOrg org);
		Task<Organization> UpdateOrganization(string org, OrgUpdate data);

		Task<QuickStart> GetQuickStart(string org);

		Task<BillingInfo> GetBillingInfo(string org);
		Task<BillingInfo> UpdatePaymentMethod(string org, string paymentMethodId);

		Task<BillingChange> PreviewBillingPlanChange(string org, Guid planId);
		Task<BillingChange> ChangeBillingPlan(string org, Guid planId);
		Task<IEnumerable<Payment>> GetPayments(string org);

		Task<Usage> GetUsage(string org);

		Task<IEnumerable<Member>> GetMembers(string org);
		Task<Member> GetMember(string org, Guid inviteToken);
		Task<Member> GetMember(string org, string userId);
		Task<Member> InviteMember(string org, NewMember data);
		Task<Member> AcceptInvite(string org, Guid memberId);
		Task<Member> UpdateMember(string org, Guid memberId, MemberUpdate data);
		Task RemoveMember(string org, Guid memberId);
	}

	public static class OrgClientExtensions
	{
		public static Task<Organization> CreateOrganization(this IOrgClient client, string name = null, string timeZone = null, Guid? planId = null, string paymentMethodId = null)
		{
			return client.CreateOrganization(new NewOrg
			{
				Name = name,
				TimeZone = timeZone,
				PlanId = planId,
				PaymentMethodId = paymentMethodId
			});
		}

		public static Task<Organization> UpdateOrgId(this IOrgClient client, string oldId, string newId)
		{
			return client.UpdateOrganization(oldId, new OrgUpdate
			{
				Id = newId
			});
		}

		public static Task<Organization> UpdateOrgName(this IOrgClient client, string org, string name)
		{
			return client.UpdateOrganization(org, new OrgUpdate
			{
				Name = name
			});
		}

		public static Task<Organization> UpdateOrgTimeZone(this IOrgClient client, string org, string timeZone)
		{
			return client.UpdateOrganization(org, new OrgUpdate
			{
				TimeZone = timeZone
			});
		}

		public static Task<Member> InviteMember(this IOrgClient client, string org, string email, MemberRole role)
		{
			return client.InviteMember(org, new NewMember
			{
				Email = email,
				Role = role
			});
		}

		public static Task<Member> UpdateMemberRole(this IOrgClient client, string org, Guid memberId, MemberRole role)
		{
			return client.UpdateMember(org, memberId, new MemberUpdate
			{
				Role = role
			});
		}

		public static async Task<BillingChange> PreviewBillingPlanChange(this IOrgClient client, string org, string planName)
		{
			var plans = await client.GetPlans();
			var plan = plans.Single(p => p.Name == planName);
			return await client.PreviewBillingPlanChange(org, plan.Id);
		}

		public static async Task<BillingChange> ChangeBillingPlan(this IOrgClient client, string org, string planName)
		{
			var plans = await client.GetPlans();
			var plan = plans.Single(p => p.Name == planName);
			return await client.ChangeBillingPlan(org, plan.Id);
		}
	}

	public class HttpOrgClient : IOrgClient
	{
		readonly HttpClient api;
		readonly IAuthenticationProvider tokenProvider;

		public HttpOrgClient(HttpClient api, IAuthenticationProvider tokenProvider)
		{
			this.api = api ?? throw new ArgumentNullException(nameof(api));
			this.tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		}

		public async Task<IEnumerable<Plan>> GetPlans(string org)
		{
			string url = "/plans" + (String.IsNullOrWhiteSpace(org) ? "" : $"?org={org}");
			var req = new HttpRequestMessage(HttpMethod.Get, url);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Plan>>();
		}

		public async Task<Organization> GetOrg(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Organization>();
		}

		public async Task<Organization> CreateOrganization(NewOrg org)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, "/orgs/").WithJsonContent(org);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Organization>();
		}

		public async Task<Organization> UpdateOrganization(string org, OrgUpdate data)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/{org}/").WithJsonContent(data);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Organization>();
		}

		public async Task<QuickStart> GetQuickStart(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/quickstart");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<QuickStart>();
		}

		public async Task<BillingInfo> GetBillingInfo(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/billing");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<BillingInfo>();
		}

		public async Task<BillingInfo> UpdatePaymentMethod(string org, string paymentMethodId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{org}/billing").WithJsonContent(new { paymentMethodId });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<BillingInfo>();
		}

		public async Task<BillingChange> PreviewBillingPlanChange(string org, Guid planId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/billing/checkout?planId={planId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<BillingChange>();
		}

		public async Task<BillingChange> ChangeBillingPlan(string org, Guid planId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{org}/billing/checkout").WithJsonContent(new { planId });
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<BillingChange>();
		}

		public async Task<IEnumerable<Payment>> GetPayments(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/payments");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Payment>>();
		}

		public async Task<Usage> GetUsage(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/usage");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Usage>();
		}

		public async Task<IEnumerable<Member>> GetMembers(string org)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/members");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<IEnumerable<Member>>();
		}

		public async Task<Member> GetMember(string org, Guid inviteToken)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/members/{inviteToken}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Member>();
		}

		public async Task<Member> GetMember(string org, string userId)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, $"/{org}/members/{Uri.EscapeDataString(userId)}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Member>();
		}

		public async Task<Member> InviteMember(string org, NewMember data)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{org}/members").WithJsonContent(data);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Member>();
		}

		public async Task<Member> AcceptInvite(string org, Guid memberId)
		{
			var req = new HttpRequestMessage(HttpMethod.Post, $"/{org}/members/{memberId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Member>();
		}

		public async Task<Member> UpdateMember(string org, Guid memberId, MemberUpdate data)
		{
			var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/{org}/members/{memberId}/").WithJsonContent(data);
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
			return await response.Content.ReadAsAsync<Member>();
		}

		public async Task RemoveMember(string org, Guid memberId)
		{
			var req = new HttpRequestMessage(HttpMethod.Delete, $"/{org}/members/{memberId}");
			req.Headers.Authorization = await tokenProvider.AcquireAuthHeader();

			var response = await api.SendAsync(req);

			await response.EnsureSuccess();
		}
	}
}
