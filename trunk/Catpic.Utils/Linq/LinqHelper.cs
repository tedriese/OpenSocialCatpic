using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Catpic.Utils.Linq
{
    using Catpic.Utils.Reflection;

    public static class LinqHelper
    {

        private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StringStartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });


        /// <summary>
        /// Builds contains expression
        /// </summary>
        static public Expression<Func<T, bool>> GetContainsPredicate<T>(ParameterExpression parameter, List<string> ids, string primaryKey)
        {
            try
            {
                //ParameterExpression entityParameter = Expression.Parameter(typeof(T), "entity");
                ConstantExpression foreignKeysParameter = Expression.Constant(ids, typeof(List<string>));
                MemberExpression memberExpression = Expression.Property(parameter, primaryKey);
                Expression convertExpression = Expression.Convert(memberExpression, typeof(string));
                MethodCallExpression containsExpression = Expression.Call(foreignKeysParameter,
                    "Contains", new Type[] { }, convertExpression);
                return Expression.Lambda<Func<T, bool>>(containsExpression, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Builds where expression from predicate
        /// </summary>
        public static MethodCallExpression GetWhere(Type entityType, Expression queryableExpr, Expression predicate)
        {
            return Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { entityType },
                queryableExpr,
                predicate);
        }

        /// <summary>
        /// Returns skip expression
        /// </summary>
        public static MethodCallExpression GetSkip(Type entityType, Expression queryableExpr, int skip)
        {
            var predicate = Expression.Constant(skip);
            return Expression.Call(
                typeof(Queryable),
                "Skip",
                new Type[] { entityType },
                queryableExpr,
                predicate);
        }

        /// <summary>
        /// Returns take expression
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="queryableExpr"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static MethodCallExpression GetTake(Type entityType, Expression queryableExpr, int take)
        {
            var predicate = Expression.Constant(take);
            return Expression.Call(
                typeof(Queryable),
                "Take",
                new Type[] { entityType },
                queryableExpr,
                predicate);
        }

        /// <summary>
        /// Returns filter predicate
        /// </summary>
        public static Expression GetFilterPredicate(string filterBy, string filterValue, string filterOp,
             ParameterExpression parameter, PropertyInfo property)
        {
            switch (filterOp)
            {
                // Return elements where filterValue exactly matches the first N characters of the element's filterBy field value, 
                // where N is the length of the filterValue string.
                case "startsWith":
                    {
                        var propertyExp = Expression.Property(parameter, property);
                        var someValue = Expression.Constant(filterValue, typeof(string));
                        return Expression.Call(propertyExp, StringStartsWithMethod, someValue);
                    }

                // Return elements where filterValue exactly matches the element's filterBy field value.
                case "equals":
                    {
                        Expression right = Expression.Constant(filterValue);
                        Expression left = Expression.Property(parameter, property);
                        return Expression.Equal(left, right);
                    }

                // Return elements where the element's filterBy field value is not empty or null.
                case "present":

                // Return elements where filterValue appears somewhere in the element's filterBy field value.
                case "contains":
                default:
                    {
                        var propertyExp = Expression.Property(parameter, property);
                        var someValue = Expression.Constant(filterValue, typeof(string));
                        return Expression.Call(propertyExp, StringContainsMethod, someValue);
                    }
            }
        }

        /// <summary>
        /// Builds orderBy expression
        /// </summary>
        /// <param name="sortBy">order property</param>
        /// <param name="sortOrder">order: ascsending or descending</param>
        /// <param name="entityType">entity type</param>
        /// <param name="whereCallExpression">where expression</param>
        /// <param name="parameter">expression parameter</param>
        /// <param name="property">entity's property</param>
        /// <returns></returns>
        public static MethodCallExpression GetOrderByExpression(string sortBy, string sortOrder, Type entityType,
            Expression whereCallExpression, ParameterExpression parameter, PropertyInfo property)
        {
            //building OrderBy expression
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            var orderOperationName = sortOrder == "descending" ? "OrderByDescending" : "OrderBy";
            return Expression.Call(
                typeof(Queryable),
                orderOperationName,
                new Type[] { entityType, property.PropertyType },
                whereCallExpression,
                Expression.Quote(orderByExp));
        }

        /// <summary>
        /// Builds anonymous type in expression 
        /// </summary>
        /// <param name="fieldNames">anonymous type properties</param>
        /// <param name="entityType">target entity type</param>
        /// <param name="expression">where and/or expression</param>
        /// <param name="properties">entity properties</param>
        /// <returns>Traget expression</returns>
        public static Expression GetFieldsExpression(IEnumerable<string> fieldNames, Type entityType, Expression expression, IEnumerable<PropertyInfo> properties)
        {
            var nameTypePropertyMapping = fieldNames.ToDictionary(name => name, name => PropertyHelper.GetPropertyByContractName(name, properties).PropertyType);
            ParameterExpression sourceItem = Expression.Parameter(entityType, "t");
            Type dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(nameTypePropertyMapping);
            IEnumerable<MemberBinding> bindings = dynamicType.GetFields().Select(p =>
                Expression.Bind(p, Expression.Property(sourceItem, PropertyHelper.GetPropertyByContractName(p.Name, properties)))).OfType<MemberBinding>();

            Expression selector = Expression.Lambda(Expression.MemberInit(
                Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem);

            // select only needed fields
            return Expression.Call(
                typeof(Queryable),
                "Select",
                new Type[] { entityType, dynamicType },
                expression, selector);
        }


        /// <summary>
        /// Gets predicate which compares two strings
        /// </summary>
        /// <param name="contractName"> The contract name. </param>
        /// <param name="constant"> The constant. </param>
        /// <param name="properties"> The properties. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <returns> Target expression</returns>
        public static Expression GetStringEqualPredicate(string contractName, string constant, IEnumerable<PropertyInfo> properties, ParameterExpression parameter)
        {
            var property = PropertyHelper.GetPropertyByContractName(contractName, properties);
            return GetStringEqualPredicate(property, constant, parameter);
        }

        /// <summary>
        /// Gets predicate which compares two strings
        /// </summary>
        /// <param name="property"> PropertyInfo instance.  </param>
        /// <param name="constant"> The constant.  </param>
        /// <param name="parameter"> The parameter.  </param>
        /// <returns> Target expression </returns>
        public static Expression GetStringEqualPredicate(PropertyInfo property, string constant, ParameterExpression parameter)
        {
            Expression right = Expression.Constant(constant);
            Expression left = Expression.Property(parameter, property);
            return Expression.Equal(left, right);
        }
    }
}
