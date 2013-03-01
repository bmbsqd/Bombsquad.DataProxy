using Bombsquad.DataProxy2.DataReaderAdaptors;

namespace Bombsquad.DataProxy.Tests.Models
{
	[MultipleResultsetContainer]
	public class FullUser
	{
		public IUser User { get; set; }
		public IMessage[] Messages { get; set; }
	}
}