using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Bombsquad.DataProxy.Utils;
using Microsoft.SqlServer.Server;

namespace Bombsquad.DataProxy.SimpleTypes
{
	public class SimpleTypeDataInfoRepository : ISimpleTypeDataInfoRepository
	{
		protected static readonly Dictionary<Type, ISimpleTypeDataInfo> m_typeMappings = new Dictionary<Type, ISimpleTypeDataInfo>
		{
			{
				typeof( bool ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Bit,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, bool>>) (( record, ordinal, value ) => record.SetBoolean( ordinal, value ))
				}
			},
			{
				typeof( byte ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.TinyInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, byte>>) (( record, ordinal, value ) => record.SetByte( ordinal, value ))
				}
			},
			{
				typeof( Int16 ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.SmallInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int16>>) (( record, ordinal, value ) => record.SetInt16( ordinal, value ))
				}
			},
			{
				typeof( char ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Char,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, char>>) (( record, ordinal, value ) => record.SetChar( ordinal, value ))
				}
			},
			{
				typeof( Int32 ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Int,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int32>>) (( record, ordinal, value ) => record.SetInt32( ordinal, value ))
				}
			},
			{
				typeof( Int64 ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.BigInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int64>>) (( record, ordinal, value ) => record.SetInt64( ordinal, value ))
				}
			},
			{
				typeof( float ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Real,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, float>>) (( record, ordinal, value ) => record.SetFloat( ordinal, value ))
				}
			},
			{
				typeof( double ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Float,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, double>>) (( record, ordinal, value ) => record.SetDouble( ordinal, value ))
				}
			},
			{
				typeof( decimal ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Decimal,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, decimal>>) (( record, ordinal, value ) => record.SetDecimal( ordinal, value ))
				}
			},
			{
				typeof( DateTime ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.DateTime,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, DateTime>>) (( record, ordinal, value ) => record.SetDateTime( ordinal, value ))
				}
			},
			{
				typeof( Guid ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.UniqueIdentifier,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Guid>>) (( record, ordinal, value ) => record.SetGuid( ordinal, value ))
				}
			},
			{
				typeof( string ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.NVarChar,
					VariableLength = true,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, string>>) (( record, ordinal, value ) => record.SetStringOrDBNull( ordinal, value ))
				}
			},
			{
				typeof( bool? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Bit,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, bool?>>) (( record, ordinal, value ) => record.SetBoolean( ordinal, value ))
				}
			},
			{
				typeof( byte? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.TinyInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, byte?>>) (( record, ordinal, value ) => record.SetByte( ordinal, value ))
				}
			},
			{
				typeof( Int16? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.SmallInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int16?>>) (( record, ordinal, value ) => record.SetInt16( ordinal, value ))
				}
			},
			{
				typeof( char? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Char,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, char?>>) (( record, ordinal, value ) => record.SetChar( ordinal, value ))
				}
			},
			{
				typeof( Int32? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Int,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int32?>>) (( record, ordinal, value ) => record.SetInt32( ordinal, value ))
				}
			},
			{
				typeof( Int64? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.BigInt,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Int64?>>) (( record, ordinal, value ) => record.SetInt64( ordinal, value ))
				}
			},
			{
				typeof( float? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Real,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, float?>>) (( record, ordinal, value ) => record.SetFloat( ordinal, value ))
				}
			},
			{
				typeof( double? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Float,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, double?>>) (( record, ordinal, value ) => record.SetDouble( ordinal, value ))
				}
			},
			{
				typeof( decimal? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.Decimal,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, decimal?>>) (( record, ordinal, value ) => record.SetDecimal( ordinal, value ))
				}
			},
			{
				typeof( DateTime? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.DateTime,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, DateTime?>>) (( record, ordinal, value ) => record.SetDateTime( ordinal, value ))
				}
			},
			{
				typeof( Guid? ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.UniqueIdentifier,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, Guid?>>) (( record, ordinal, value ) => record.SetGuid( ordinal, value ))
				}
			},
			{
				typeof( byte[] ), new SimpleTypeDataInfo
				{
					SqlDbType = SqlDbType.VarBinary,
					VariableLength = true,
					SetSqlDataRecordValueExpression = (Expression<Action<SqlDataRecord, int, byte[]>>) (( record, ordinal, value ) => record.SetBytes( ordinal, 0, value, 0, value.Length ))
				}
			}
		};

		public bool TryGetTypeMapping( Type type, out ISimpleTypeDataInfo simpleTypeDataInfo )
		{
			return m_typeMappings.TryGetValue( type, out simpleTypeDataInfo );
		}
	}
}