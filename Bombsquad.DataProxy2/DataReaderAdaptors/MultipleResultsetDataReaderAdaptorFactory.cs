using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Bombsquad.DataProxy2.DataReaderAdaptors
{
    public class MultipleResultsetDataReaderAdaptorFactory : IDataReaderAdaptorFactory
    {
        public bool TryCreate<T>(IDataProxyContext context, out IDataReaderAdaptor<T> dataReaderAdaptor)
        {
            var attribute = typeof(T).GetCustomAttribute<MultipleResultsetContainerAttribute>();
            if (attribute == null)
            {
                dataReaderAdaptor = null;
                return false;
            }

            dataReaderAdaptor = new MultipleResultsetDataReaderAdaptor<T>(CreateInstanceFactory<T>(), CreateResultsetReaders<T>(context).ToArray());
            return true;
        }

        private IEnumerable<IResultsetReader<T>> CreateResultsetReaders<T>(IDataProxyContext context)
        {
            var createResultsetGenericMethod = GetType().GetMethod("CreateResultsetReader", BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.GetSetMethod() != null)
                {
                    var createResultsetMethod = createResultsetGenericMethod.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
                    yield return (IResultsetReader<T>)createResultsetMethod.Invoke(null, new object[] { context, propertyInfo });
                }
            }
        }

        private static Func<T> CreateInstanceFactory<T>()
        {
            var constructor = typeof(T).GetConstructor(new Type[0]);
            return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
        }

        private static IResultsetReader<T> CreateResultsetReader<T, TV>(IDataProxyContext context, PropertyInfo property)
        {
            var readerAdaptor = context.GetOrCreateDataReaderAdaptor<TV>();
            var instanceParameter = Expression.Parameter(typeof(T), "instance");
            var valueParameter = Expression.Parameter(typeof(TV), "value");
            var valueSetterAction = Expression.Lambda<Action<T, TV>>(Expression.Assign(Expression.Property(instanceParameter, property), valueParameter), instanceParameter, valueParameter).Compile();
            return new ResultsetReader<T, TV>(valueSetterAction, readerAdaptor);
        }

        private class MultipleResultsetDataReaderAdaptor<T> : IDataReaderAdaptor<T>
        {
            private readonly Func<T> m_instanceFactory;
            private readonly IEnumerable<IResultsetReader<T>> m_resultsetReaders;

            public MultipleResultsetDataReaderAdaptor(Func<T> instanceFactory, IEnumerable<IResultsetReader<T>> resultsetReaders)
            {
                m_instanceFactory = instanceFactory;
                m_resultsetReaders = resultsetReaders;
            }

            public T Read(IExecuteReaderContext connectionAndReaderContext)
            {
                var instance = m_instanceFactory();

                using (connectionAndReaderContext)
                {
                    connectionAndReaderContext.OpenConnectionAndExecuteReader();
                    foreach (var resultsetReader in m_resultsetReaders)
                    {
                        resultsetReader.ReadResultsetAndSetProperty(instance, connectionAndReaderContext);

                        if (!connectionAndReaderContext.Reader.NextResult())
                        {
                            break;
                        }
                    }
                }

                return instance;
            }

            public async Task<T> ReadAsync(IExecuteReaderContext connectionAndReaderContext)
            {
                var instance = m_instanceFactory();

                using (connectionAndReaderContext)
                {
                    await connectionAndReaderContext.OpenConnectionAndExecuteReaderAsync();
                    foreach (var resultsetReader in m_resultsetReaders)
                    {
                        await resultsetReader.ReadResultsetAndSetPropertyAsync(instance, connectionAndReaderContext);

                        if (!await connectionAndReaderContext.Reader.NextResultAsync())
                        {
                            break;
                        }
                    }
                }

                return instance;
            }
        }

        private interface IResultsetReader<T>
        {
            void ReadResultsetAndSetProperty(T instance, IExecuteReaderContext connectionAndReaderContext);
            Task ReadResultsetAndSetPropertyAsync(T instance, IExecuteReaderContext connectionAndReaderContext);
        }

        private class ResultsetReader<T, TV> : IResultsetReader<T>
        {
            private readonly Action<T, TV> m_valueSetter;
            private readonly IDataReaderAdaptor<TV> m_dataReaderAdaptor;

            public ResultsetReader(Action<T, TV> valueSetter, IDataReaderAdaptor<TV> dataReaderAdaptor)
            {
                m_valueSetter = valueSetter;
                m_dataReaderAdaptor = dataReaderAdaptor;
            }

            public void ReadResultsetAndSetProperty(T instance, IExecuteReaderContext connectionAndReaderContext)
            {
                var value = m_dataReaderAdaptor.Read(new NonDisposingExecuteReaderContext(connectionAndReaderContext));
                m_valueSetter(instance, value);
            }

            public async Task ReadResultsetAndSetPropertyAsync(T instance, IExecuteReaderContext connectionAndReaderContext)
            {
                var value = await m_dataReaderAdaptor.ReadAsync(new NonDisposingExecuteReaderContext(connectionAndReaderContext));
                m_valueSetter(instance, value);
            }

            private class NonDisposingExecuteReaderContext : IExecuteReaderContext
            {
                private readonly IExecuteReaderContext m_inner;

                public NonDisposingExecuteReaderContext(IExecuteReaderContext inner)
                {
                    m_inner = inner;
                }

                public void Dispose()
                {
                }

                public SqlDataReader Reader
                {
                    get
                    {
                        return m_inner.Reader;
                    }
                }

                public void OpenConnectionAndExecuteReader()
                {
                    m_inner.OpenConnectionAndExecuteReader();
                }

                public Task OpenConnectionAndExecuteReaderAsync()
                {
                    return m_inner.OpenConnectionAndExecuteReaderAsync();
                }
            }
        }
    }
}