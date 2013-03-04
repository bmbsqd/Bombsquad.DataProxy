using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Bombsquad.DataProxy.ColumnValueConverters
{
	public class NullableColumnValueConverterFactory : IColumnValueConverterFactory
	{
		private readonly MethodInfo m_createMethod;

		public NullableColumnValueConverterFactory()
		{
			m_createMethod = GetType().GetMethod( "CreateNullableColumnValueConverter", BindingFlags.NonPublic | BindingFlags.Static );
		}

		public bool TryCreate<T>( IDataProxyContext context, out IColumnValueConverter<T> converter )
		{
			if(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var method = m_createMethod.MakeGenericMethod( typeof( T ).GetGenericArguments().Single() );
				converter = (IColumnValueConverter<T>) method.Invoke( this, new object[] { context } );
				return true;
			}

			converter = null;
			return false;
		}

		private static NullableColumnValueConverter<TValue> CreateNullableColumnValueConverter<TValue>( IDataProxyContext context ) where TValue : struct
		{
			return new NullableColumnValueConverter<TValue>( context.GetOrCreateColumnValueConverter<TValue>() );
		} 

		private class NullableColumnValueConverter<TValue> : IColumnValueConverter<TValue?> where TValue : struct
		{
			private readonly IColumnValueConverter<TValue> m_innerConverter;

			public NullableColumnValueConverter(IColumnValueConverter<TValue> innerConverter)
			{
				m_innerConverter = innerConverter;
			}

			public TValue? Read( SqlDataReader reader, int ordinal )
			{
				return reader.IsDBNull( ordinal ) ? (TValue?) null : m_innerConverter.Read( reader, ordinal );
			}

			public async Task<TValue?> ReadAsync( SqlDataReader reader, int ordinal )
			{
				return await reader.IsDBNullAsync( ordinal ) ? (TValue?) null : await m_innerConverter.ReadAsync( reader, ordinal );
			}
		}
	}
}