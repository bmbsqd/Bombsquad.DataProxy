using System;

namespace Bombsquad.DataProxy.CommandInformation
{
	[AttributeUsage( AttributeTargets.Parameter )]
	public class IgnoredParameterAttribute : Attribute
	{
	}
}