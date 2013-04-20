// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryTranslator.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Wraps Entity Framework to intercept expression before execution
//   see  http://stackoverflow.com/questions/1839901/how-to-wrap-entity-framework-to-intercept-the-linq-expression-just-before-execut
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Data.EntityFramework.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Wraps Entity Framework to intercept expression before execution
    /// </summary>
    /// <typeparam name="T"> Entity type </typeparam>
    public class QueryTranslator<T> : IOrderedQueryable<T>
    {
        private Expression expression = null;
        private QueryTranslatorProvider<T> provider = null;

        public QueryTranslator(IQueryable source)
        {
            this.expression = Expression.Constant(this);
            this.provider = new QueryTranslatorProvider<T>(source);
        }

        public QueryTranslator(IQueryable source, Expression e)
        {
            if (e == null) throw new ArgumentNullException("e");
            this.expression = e;
            this.provider = new QueryTranslatorProvider<T>(source);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.ExecuteEnumerable(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.provider.ExecuteEnumerable(this.expression).GetEnumerator();
        }

        public QueryTranslator<T> Include(String path)
        {
            ObjectQuery<T> possibleObjectQuery = this.provider.source as ObjectQuery<T>;
            if (possibleObjectQuery != null)
            {
                return new QueryTranslator<T>(possibleObjectQuery.Include(path));
            }
            else
            {
                throw new InvalidOperationException("The Include should only happen at the beginning of a LINQ expression");
            }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return this.expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.provider; }
        }
    }

    public class QueryTranslatorProvider<T> : ExpressionVisitor, IQueryProvider
    {
        internal IQueryable source;

        public QueryTranslatorProvider(IQueryable source)
        {
            if (source == null) throw new ArgumentNullException("source");
            this.source = source;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            return new QueryTranslator<TElement>(this.source, expression) as IQueryable<TElement>;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            Type elementType = expression.Type.GetGenericArguments().First();
            IQueryable result = (IQueryable)Activator.CreateInstance(typeof(QueryTranslator<>).MakeGenericType(elementType),
                    new object[] { this.source, expression });
            return result;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            object result = (this as IQueryProvider).Execute(expression);
            return (TResult)result;
        }

        public object Execute(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            Expression translated = this.Visit(expression);
            return this.source.Provider.Execute(translated);
        }

        internal IEnumerable ExecuteEnumerable(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

            Expression translated = this.Visit(expression);
            return this.source.Provider.CreateQuery(translated);
        }

        #region Visitors
        protected override Expression VisitConstant(ConstantExpression c)
        {
            // fix up the Expression tree to work with EF again
            if (c.Type == typeof(QueryTranslator<T>))
            {
                return this.source.Expression;
            }
            else
            {
                return base.VisitConstant(c);
            }
        }
        #endregion
    }
}
