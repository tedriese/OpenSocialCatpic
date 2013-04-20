using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Catpic.Social;
using Catpic.Social.People;
using Catpic.Utils;

namespace Catpic.Host.Engine.Social
{
    public class PeopleRepository : IRepository<Person>
    {
        private readonly IList<EntityCollection<Person>> _people;
        private IQueryable<EntityCollection<Person>> _queryable;

        public PeopleRepository(IList<EntityCollection<Person>> people)
        {
            _people = people;
            _queryable = _people.AsQueryable();
        }

        public IQueryable GetQueryable()
        {
            return _queryable;
        }

        public Task<Person> AddEntityAsync(string userId, string collectionId, Person person)
        {
            var collection = _people.Single(p => p.UserId == userId && p.Type == "@friends").Entities;
            var friend =_people.Single(p => p.UserId == person.Id && p.Type == "@self").Entities.Single();
            (collection as IList<Person>).Add(friend);
            return AsyncHelper.GetEmptyTask(friend);
        }

        public Task<Person> UpdateEntityAsync(string userId, string collectionId, Person entity)
        {
            var person = _people.FirstOrDefault(p => p.UserId == userId && p.Type == "@self").Entities.Single();

            // NOTE: dummy implementation for unit testing
            person.DisplayName = entity.DisplayName;
            return AsyncHelper.GetEmptyTask(person);
        }

        public Task<Person> DeleteEntityAsync(string userId, string collectionId, Person entity)
        {
            // TODO if @self remove person
            var collection = _people.Single(p => p.UserId == userId && p.Type == collectionId);
            (collection.Entities as List<Person>).RemoveAll(p => p.Id == entity.Id);
            return AsyncHelper.GetEmptyTask(entity);
        }

        public Task<string> AddCollectionAsync(EntityCollection<Person> collection)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCollectionAsync(EntityCollection<Person> collectione)
        {
            throw new NotImplementedException();
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
    }
}