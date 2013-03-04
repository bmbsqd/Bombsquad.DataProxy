using System;
using Microsoft.SqlServer.Server;

namespace Bombsquad.DataProxy.Utils
{
	public static class SqlDataRecordExtensions
	{
		public static void SetStringOrDBNull( this SqlDataRecord record, int ordinal, string value )
		{
			if ( value != null )
				record.SetString( ordinal, value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetBoolean( this SqlDataRecord record, int ordinal, bool? value )
		{
			if ( value != null )
				record.SetBoolean( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetByte( this SqlDataRecord record, int ordinal, byte? value )
		{
			if ( value != null )
				record.SetByte( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetInt16( this SqlDataRecord record, int ordinal, short? value )
		{
			if ( value != null )
				record.SetInt16( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetInt32( this SqlDataRecord record, int ordinal, int? value )
		{
			if ( value != null )
				record.SetInt32( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetInt64( this SqlDataRecord record, int ordinal, long? value )
		{
			if ( value != null )
				record.SetInt64( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetChar( this SqlDataRecord record, int ordinal, char? value )
		{
			if ( value != null )
				record.SetChar( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetFloat( this SqlDataRecord record, int ordinal, float? value )
		{
			if ( value != null )
				record.SetFloat( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetDouble( this SqlDataRecord record, int ordinal, double? value )
		{
			if ( value != null )
				record.SetDouble( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetDecimal( this SqlDataRecord record, int ordinal, decimal? value )
		{
			if ( value != null )
				record.SetDecimal( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetDateTime( this SqlDataRecord record, int ordinal, DateTime? value )
		{
			if ( value != null )
				record.SetDateTime( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}

		public static void SetGuid( this SqlDataRecord record, int ordinal, Guid? value )
		{
			if ( value != null )
				record.SetGuid( ordinal, value.Value );
			else
				record.SetDBNull( ordinal );
		}
	}
}