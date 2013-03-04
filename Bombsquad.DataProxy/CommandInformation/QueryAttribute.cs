using System;
using System.Data;

namespace Bombsquad.DataProxy.CommandInformation
{
	[AttributeUsage(AttributeTargets.Method)]
	public class QueryAttribute : Attribute
	{
		public QueryAttribute(string commandText) 
			: this( commandText, CommandType.StoredProcedure )
		{
		}

		public QueryAttribute(string commandText, CommandType commandType)
		{
			CommandText = commandText;
			CommandType = commandType;
		}

		public string CommandText
		{
			get;
			set;
		}

		public CommandType CommandType
		{
			get;
			set;
		}
	}
}