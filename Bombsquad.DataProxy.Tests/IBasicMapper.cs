using System;
using Bombsquad.DataProxy.CommandInformation;

namespace Bombsquad.DataProxy.Tests
{
	public interface IBasicMapper
	{
		[Query( "OutputParameterTest" )]
		void OutputParameterTest( int input, out int output );

		[Query( "DataTypesTest" )]
		IDataTypeTestResult DataTypesTest( bool bitValue, byte tinyIntValue, short smallIntValue, int intValue, long bigIntValue, float realValue, double floatValue, decimal decimalValue, DateTime dateTimeValue, string stringValue, Guid guidValue );

		[Query( "DataTypesTest" )]
		INullableDataTypeTestResult NullableDataTypesTest( bool? bitValue, byte? tinyIntValue, short? smallIntValue, int? intValue, long? bigIntValue, float? realValue, double? floatValue, decimal? decimalValue, DateTime? dateTimeValue, string stringValue, Guid? guidValue );
	}
}