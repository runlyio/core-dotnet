using Dapper;
using Examples.WebApp.Web.Models;
using Examples.WebApp.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Examples.WebApp.Web.Controllers
{
	public class InviteController : Controller
	{
		readonly DbConnection db;
		readonly IJobQueue jobs;

		public InviteController(DbConnection db, IJobQueue jobs)
		{
			this.db = db;
			this.jobs = jobs;
		}

		public IActionResult Index() => View();

		[HttpPost]
		public async Task<IActionResult> InviteUsers(InvitationModel data)
		{
			// Do the critical path work: actually get
			// the users/emails into the system.
			await InsertUsersIntoDatabase(data.EmailList);

			// Then queue the non-critical path work
			// in a separate async-job: email
			// invitations will be sent to each user.
			// This also makes the job of sending
			// emails fault tolerant in case of
			// temporary errors while sending a message.
			Guid runId = await jobs.SendPendingInvitations();

			return View("Results", new RunResultsModel
			{
				RunId = runId
			});
		}

		async Task InsertUsersIntoDatabase(IEnumerable<string> emails)
		{
			await db.OpenAsync();

			using (var tx = await db.BeginTransactionAsync())
			{
				foreach (string email in emails)
				{
					await db.ExecuteAsync("insert into [User] (Email) values (@Email)", new { email }, tx);
				}

				await tx.CommitAsync();
			}
		}
	}
}
