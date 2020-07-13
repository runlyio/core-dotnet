using System;
using System.Collections.Generic;

namespace Examples.WebApp.Web.Models
{
	public class InvitationModel
	{
		public string Emails { get; set; }

		// split input on new lines
		public IEnumerable<string> EmailList => Emails.Split(
			new[] { "\r\n", "\r", "\n" },
			StringSplitOptions.RemoveEmptyEntries
		);
	}
}
