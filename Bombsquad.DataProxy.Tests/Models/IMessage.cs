using System;

namespace Bombsquad.DataProxy.Tests.Models
{
	public interface IMessage
	{
		int MessageId { get; }
		int ToUserId { get; }
		int FromUserId { get; }
		DateTime SentDate { get; }
		string Subject { get; }
		string Body { get; }
	}
}