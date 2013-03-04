using System;

namespace Bombsquad.DataProxy.SimpleTypes
{
	public interface ISimpleTypeDataInfoRepository
	{
		bool TryGetTypeMapping( Type type, out ISimpleTypeDataInfo simpleTypeDataInfo );
	}
}