// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocialExpressionFactory.cs" company="Catpic Software">
//   Licensed under Apache License 2.0
// </copyright>
// <summary>
//   Provides basic functionality to build expressions to social services
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Catpic.Social
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    
    using Catpic.Social.Formatting;
    using Catpic.Utils.Linq;
    using Catpic.Utils.Reflection;

    /// <summary>
    /// Provides basic functionality to build expressions to social services
    /// </summary>
    /// <typeparam name="T"> Entity type </typeparam>
    public class SocialExpressionFactory<T> where T : IIdentityField
    {
        /// <summary>
        /// Creates expression which selects entities from a given collection
        /// </summary>
        /// <param name="userId"> The user id. </param>
        /// <param name="collectionId"> Enity collection id. </param>
        /// <param name="collectionItem"> The request item. </param>
        /// <param name="queryableExpr"> The queryable expr. </param>
        /// <returns> Target expression </returns>
        public virtual Expression CreateEntityListExpression(string userId, string collectionId, CollectionItem collectionItem, Expression queryableExpr)
        {
            Type collectionType = typeof(EntityCollection<T>);
            ParameterExpression parameter = Expression.Parameter(collectionType, "collection");
            var properties = collectionType.GetProperties();

            var left = LinqHelper.GetStringEqualPredicate("id", collectionId, properties, parameter);
            var right = LinqHelper.GetStringEqualPredicate(properties.Single(p => p.Name == "UserId"), userId, parameter);

            var predicate = Expression.Lambda<Func<EntityCollection<T>, bool>>(
                Expression.And(left, right), new[] { parameter });

            return this.GetSelectExpression(collectionItem, queryableExpr, predicate, collectionType, parameter, properties, true);
        }

        /// <summary>
        /// Creates expression which selects certain entity from a given collection
        /// </summary>
        /// <param name="userId"> The user id. </param>
        /// /// <param name="collectionId"> Entity collection id. </param>
        /// <param name="entityId"> Entity id. </param>
        /// <param name="collectionItem"> The request item. </param>
        /// <param name="queryableExpr"> The queryable expr. </param>
        /// <returns> Target expression </returns>
        public virtual Expression CreateEntityExpression(string userId, string collectionId, string entityId, CollectionItem collectionItem, Expression queryableExpr)
        {
            collectionItem.FilterBy = "id";
            collectionItem.FilterOp = "equals";
            collectionItem.FilterValue = entityId;
            return this.CreateEntityListExpression(userId, collectionId, collectionItem, queryableExpr);
        }

        /// <summary>
        /// Gets expression for entity collection
        /// </summary>
        /// <param name="userId"> User id. </param>
        /// <param name="collectionItem"> Collection item. </param>
        /// <param name="queryableExpr"> The queryable expr. </param>
        /// <returns> Target expression</returns>
        public virtual Expression CreateCollectionListExpression(string userId, CollectionItem collectionItem, Expression queryableExpr)
        {
            Type collectionType = typeof(EntityCollection<T>);
            ParameterExpression parameter = Expression.Parameter(collectionType, "collection");
            var properties = collectionType.GetProperties();

            var right = LinqHelper.GetStringEqualPredicate(properties.Single(p => p.Name == "UserId"), userId, parameter);
            var predicate = Expression.Lambda<Func<EntityCollection<T>, bool>>(right, new[] { parameter });

            return this.GetSelectExpression(collectionItem, queryableExpr, predicate, collectionType, parameter, properties, false);
        }

        /// <summary>
        /// Gets select expression
        /// </summary>
        /// <param name="collectionItem"> The collection item. </param>
        /// <param name="queryableExpr"> The queryable expr. </param>
        /// <param name="predicate"> The predicate. </param>
        /// <param name="collectionType"> The entity type. </param>
        /// <param name="parameter"> The parameter. </param>
        /// <param name="properties"> The properties. </param>
        /// <param name="isExpand"> True if need return entities. </param>
        /// <returns> Target expression </returns>
        protected virtual Expression GetSelectExpression(CollectionItem collectionItem, Expression queryableExpr, Expression predicate, Type collectionType, ParameterExpression parameter, IEnumerable<PropertyInfo> properties, bool isExpand = false)
        {
            var expression = LinqHelper.GetWhere(collectionType, queryableExpr, predicate);
            if (isExpand)
            {
                var entityType = typeof(T);
                var entityProperties = entityType.GetProperties();

                ParameterExpression entityParameter = Expression.Parameter(entityType, "entity");
                var entityProperty = properties.Single(p => p.Name == "Entities");

                // entity.Messages
                var propertyExp = Expression.Property(parameter, entityProperty);

                // entity => entity.Messages
                var entityExpression = Expression.Lambda<Func<EntityCollection<T>, IEnumerable<T>>>(propertyExp, new[] { parameter });

                // attach SelectMany:
                // .Where(entity => ((entity.Id == "notification") And (entity.UserId == "john.doe"))).SelectMany(entity => entity.Messages)
                expression = Expression.Call(
                    typeof(Queryable),
                    "SelectMany",
                    new[] { collectionType, entityType },
                    expression,
                    entityExpression);

                // need filter by message properties
                var predicateProperty = PropertyHelper.GetPropertyByContractName(collectionItem.FilterBy, entityProperties);
                if (!string.IsNullOrEmpty(collectionItem.FilterBy) && predicateProperty != null)
                {
                    // filter predicate: m.Title.Contains("o")
                    Expression filter = LinqHelper.GetFilterPredicate(
                        collectionItem.FilterBy,
                        collectionItem.FilterValue,
                        collectionItem.FilterOp,
                        entityParameter,
                        predicateProperty);

                    // wrap:  m => m.Title.Contains("o")
                    var filterWhere = Expression.Lambda<Func<T, bool>>(filter, new[] { entityParameter });

                    // attach where to expression:
                    // .Where(entity => ((entity.Id == "notification") And (entity.UserId == "john.doe"))).SelectMany(entity => entity.Messages).Where(message => message.Title.Contains("o"))
                    expression = Expression.Call(
                       typeof(Queryable),
                       "Where",
                       new[] { entityType },
                       expression,
                       filterWhere);
                }

                // NOTE: right now we return different type
                collectionType = entityType;
                properties = entityProperties;
                parameter = entityParameter;
            }

            // get orderBy expression
            if (!string.IsNullOrEmpty(collectionItem.SortBy))
            {
                var property = PropertyHelper.GetPropertyByContractName(collectionItem.SortBy, properties);
                if (property != null)
                {
                    expression = LinqHelper.GetOrderByExpression(
                        collectionItem.SortBy, collectionItem.SortOrder, collectionType, expression, parameter, property);
                }
                else
                {
                    collectionItem.SortBy = string.Empty;
                }
            }

            // paging 
            expression = LinqHelper.GetSkip(collectionType, expression, collectionItem.StartIndex);
            expression = LinqHelper.GetTake(collectionType, expression, collectionItem.Count);

            // return full entity if fields isn't specified)
            if (collectionItem.Fields == null)
            {
                return expression;
            }

            // build anonymous type in expression 
            var fieldNames = collectionItem.Fields.Distinct();
            try
            {
                return LinqHelper.GetFieldsExpression(fieldNames, collectionType, expression, properties);
            }
            catch
            {
                return expression;
            }
        }
    }
}
