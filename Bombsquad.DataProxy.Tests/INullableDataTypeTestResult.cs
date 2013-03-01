using System;

namespace Bombsquad.DataProxy.Tests
{
	public interface INullableDataTypeTestResult
	{
		bool? BitValue { get; }
		byte? TinyIntValue { get; }
		short? SmallIntValue { get; }
		int? IntValue { get; }
		long? BigIntValue { get; }
		float? RealValue { get; }
		double? FloatValue { get; }
		decimal? DecimalValue { get; }
		DateTime? DateTimeValue { get; }
		string StringValue { get; }
		Guid? GuidValue { get; }
	}
}