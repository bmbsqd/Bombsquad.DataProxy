using System;

namespace Bombsquad.DataProxy2.SimpleTypes
{
	public interface ISimpleTypeDataInfoRepository
	{
		bool TryGetTypeMapping( Type type, out ISimpleTypeDataInfo simpleTypeDataInfo );
	}
}