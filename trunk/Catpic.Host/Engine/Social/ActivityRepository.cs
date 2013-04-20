using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Catpic.Social.Activities;
using Catpic.Utils;

namespace Catpic.Host.Engine.Social
{
    using Catpic.Social;

    /// <summary>
    /// Provides default implementation of activities/activity streams repositories
    /// </summary>
    public class ActivityRepository: IRepository<Activity>
    {
        private readonly IList<EntityCollection<Activity>> _activities;
        private IQueryable<EntityCollection<Activity>> _queryable;

        public ActivityRepository(IList<EntityCollection<Activity>> activities)
         {
             _activities = activities;
             _queryable = activities.AsQueryable();
         }

        public IQueryable GetQueryable()
        {
            return _queryable;
        }

        public Task<Activity> AddEntityAsync(string userId, string collectionId, Activity activity)
        {
            var collection = _activities.Single(c => c.UserId == userId && c.Type == collectionId);

            activity.Id = GetId(collection.Entities.Last());
            activity.UserId = userId;
            var list = collection.Entities.ToList();
            list.Add(activity);
            collection.Entities = list;

            _queryable = _activities.AsQueryable();
            return AsyncHelper.GetEmptyTask(activity);
        }

        public Task<Activity> UpdateEntityAsync(string userId, string collectionId, Activity entity)
        {
            throw new NotImplementedException();
        }

        public Task<Activity> DeleteEntityAsync(string userId, string collectionId, Activity entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> AddCollectionAsync(EntityCollection<Activity> collection)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCollectionAsync(EntityCollection<Activity> collection)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteCollectionAsync(string userId, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable> SelectAsync(Expression expression)
        {
            return AsyncHelper.GetEmptyTask<IQueryable>(_queryable.Provider.CreateQuery(expression));
        }

        public Task<IEnumerable<object>> Select(Expression expression)
        {
            var query = _queryable.Provider.CreateQuery(expression);
            return AsyncHelper.GetEmptyTask(query as IEnumerable<object>);
        }

        private string GetId(Activity last)
        {
            // USED ONLY FOR UNIT TESTING!
            var id = last.Id;
            int number;
            if (Int32.TryParse(id, out number))
                return number.ToString();
            if (id.StartsWith("activity"))
            {
                return (int.Parse(id.TrimStart("activity".ToCharArray())) + 1).ToString();
            }

            throw new InvalidOperationException("Unable to generate valid unit testing id");
        }
    }
}