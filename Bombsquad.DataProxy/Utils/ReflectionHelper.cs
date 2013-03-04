using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bombsquad.DataProxy.Utils
{
	internal static class ReflectionHelper
	{
		internal static bool TryGetCustomAttribute<TAttribute>( this ICustomAttributeProvider customAttributeProvider, out TAttribute attribute ) where TAttribute : Attribute
		{
			attribute = customAttributeProvider.GetCustomAttributes( typeof( TAttribute ), false ).FirstOrDefault() as TAttribute;
			return attribute != null;
		}

		internal static MethodInfo GetMethodInfo<TInstance>( Expression<Action<TInstance>> expression )
		{
			return GetMethodInfoFromExpression( expression );
		}

		internal static PropertyInfo GetProperty<T,TResult>( Expression<Func<T, TResult>> propertySelector )
		{
			Expression body = propertySelector;
			if ( body is LambdaExpression )
			{
				body = ((LambdaExpression) body).Body;
			}
			switch ( body.NodeType )
			{
				case ExpressionType.MemberAccess:
					return (PropertyInfo) ((MemberExpression) body).Member;
				default:
					throw new InvalidOperationException();
			}
		}

		public static MethodInfo GetMethodInfoFromExpression( Expression method )
		{
			var lambda = method as LambdaExpression;
			if ( lambda == null )
			{
				throw new ArgumentNullException( "method" );
			}

			MethodCallExpression methodExpr = null;

			// Our Operation<T> returns an object, so first statement can be either
			// a cast (if method does not return an object) or the direct method call.
			if ( lambda.Body.NodeType == ExpressionType.Convert )
			{
				// The cast is an unary expression, where the operand is the
				// actual method call expression.
				methodExpr = ((UnaryExpression) lambda.Body).Operand as MethodCallExpression;
			}
			else if ( lambda.Body.NodeType == ExpressionType.Call )
			{
				methodExpr = lambda.Body as MethodCallExpression;
			}

			if ( methodExpr == null )
			{
				throw new ArgumentException( "method" );
			}

			return methodExpr.Method;
		}

		internal static bool TryGetIEnumerableElementType( Type type, out Type elementType )
		{
			if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( IEnumerable<> ) )
			{
				elementType = type.GetGenericArguments().Single();
				return true;
			}

			foreach ( var i in type.GetInterfaces() )
			{
				if ( TryGetIEnumerableElementType( i, out elementType ) )
				{
					return true;
				}
			}

			elementType = null;
			return false;
		}
	}
}