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
    public class ActivityStreamsRepository: IRepository<ActivityEntry>
    {
        private readonly IList<EntityCollection<ActivityEntry>> _activities;
        private IQueryable<EntityCollection<ActivityEntry>> _queryable;

        public ActivityStreamsRepository(IList<EntityCollection<ActivityEntry>> activities)
         {
             _activities = activities;
             _queryable = activities.AsQueryable();
         }

        public IQueryable GetQueryable()
        {
            return _queryable;
        }

        public Task<IQueryable> SelectAsync(Expression expression)
        {
            return AsyncHelper.GetEmptyTask<IQueryable>(_queryable.Provider.CreateQuery(expression));
        }

        public Task<string> DeleteCollectionAsync(string userId, string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<object>> Select(Expression expression)
        {
            var query = _queryable.Provider.CreateQuery(expression);
            return AsyncHelper.GetEmptyTask(query as IEnumerable<object>);
        }


        public Task<ActivityEntry> AddEntityAsync(string userId, string collectionId, ActivityEntry activity)
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

        public Task<ActivityEntry> UpdateEntityAsync(string userId, string collectionId, ActivityEntry entity)
        {
            var collection = _activities.Single(p => p.UserId == userId && p.Type == collectionId);
            var activity = collection.Entities.Single(a => a.Id == entity.Id);
            // NOTE: dummy implementation for unit testing
            activity.Title = entity.Title;
            return AsyncHelper.GetEmptyTask(activity);
        }

        public Task<ActivityEntry> DeleteEntityAsync(string userId, string collectionId, ActivityEntry entity)
        {
            var collection = _activities.Single(p => p.UserId == userId && p.Type == collectionId);
            var activity = collection.Entities.Single(a => a.Id == entity.Id);
            (collection.Entities as List<ActivityEntry>).Remove(activity);

            return AsyncHelper.GetEmptyTask(activity);
        }

        public Task<string> AddCollectionAsync(EntityCollection<ActivityEntry> collection)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCollectionAsync(EntityCollection<ActivityEntry> collection)
        {
            throw new NotImplementedException();
        }

        private string GetId(ActivityEntry last)
        {
            // USED ONLY FOR UNIT TESTING!
            var id = last.Id;
            int number;
            if(Int32.TryParse(id, out number)) 
                return number.ToString();
            if(id.StartsWith("activity"))
            {
                return (int.Parse(id.TrimStart("activity".ToCharArray()))+1).ToString();
            }

            throw new InvalidOperationException("Unable to generate valid unit testing id");
        }
    }
}