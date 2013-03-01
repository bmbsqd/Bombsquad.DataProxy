using System;

namespace Bombsquad.DataProxy2.CommandInformation
{
	[AttributeUsage( AttributeTargets.Parameter )]
	public class IgnoredParameterAttribute : Attribute
	{
	}
}