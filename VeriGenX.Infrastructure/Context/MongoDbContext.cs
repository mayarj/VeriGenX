using MongoDB.Driver;
using MongoDB.Bson;



namespace VeriGenX.Infrastructure.Context
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private IClientSessionHandle _session;

        public MongoDbContext(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string? name = null)
        {
            return _database.GetCollection<T>(name ?? typeof(T).Name);
        }

        #region Transaction Support
        public async Task BeginTransactionAsync()
        {
            _session = await _client.StartSessionAsync();
            _session.StartTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            if (_session?.IsInTransaction ?? false)
            {
                await _session.CommitTransactionAsync();
            }
            _session?.Dispose();
            _session = null;
        }

        public async Task AbortTransactionAsync()
        {
            if (_session?.IsInTransaction ?? false)
            {
                await _session.AbortTransactionAsync();
            }
            _session?.Dispose();
            _session = null;
        }

        public bool IsInTransaction => _session?.IsInTransaction ?? false;
        #endregion

        #region Index Management
        public async Task CreateIndexAsync<T>(string field, CreateIndexOptions options = null, string collectionName = null)
        {
            var indexKeys = Builders<T>.IndexKeys.Ascending(field);
            await GetCollection<T>(collectionName).Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeys, options));
        }

        public async Task CreateUniqueIndexAsync<T>(string field, string collectionName = null)
        {
            await CreateIndexAsync<T>(field, new CreateIndexOptions { Unique = true }, collectionName);
        }

        public async Task CreateCompoundIndexAsync<T>(IEnumerable<string> fields, CreateIndexOptions options = null, string collectionName = null)
        {
            var indexBuilder = Builders<T>.IndexKeys;
            var indexKeys = fields.Aggregate(indexBuilder.Combine(), (current, field) => current.Ascending(field));
            await GetCollection<T>(collectionName).Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeys, options));
        }
        #endregion

        #region Helper Methods
        public async Task<bool> CollectionExistsAsync<T>(string collectionName = null)
        {
            collectionName = collectionName ?? typeof(T).Name;
            var filter = new BsonDocument("name", collectionName);
            var collections = await _database.ListCollectionNamesAsync(new ListCollectionNamesOptions { Filter = filter });
            return await collections.AnyAsync();
        }

        public async Task DropCollectionAsync<T>(string collectionName = null)
        {
            collectionName = collectionName ?? typeof(T).Name;
            await _database.DropCollectionAsync(collectionName);
        }

        public async Task<long> CountDocumentsAsync<T>(FilterDefinition<T> filter = null, string collectionName = null)
        {
            filter ??= FilterDefinition<T>.Empty;
            return await GetCollection<T>(collectionName).CountDocumentsAsync(filter);
        }

        public async Task<bool> AnyAsync<T>(FilterDefinition<T> filter = null, string collectionName = null)
        {
            filter ??= FilterDefinition<T>.Empty;
            return await GetCollection<T>(collectionName).Find(filter).AnyAsync();
        }
        #endregion

        #region Bulk Operations
        public async Task BulkInsertAsync<T>(IEnumerable<T> documents, string collectionName = null)
        {
            var collection = GetCollection<T>(collectionName);
            await collection.InsertManyAsync(documents);
        }

        public async Task BulkUpdateAsync<T>(IEnumerable<WriteModel<T>> operations, string collectionName = null, BulkWriteOptions options = null)
        {
            var collection = GetCollection<T>(collectionName);
            await collection.BulkWriteAsync(operations, options);
        }
        #endregion

        #region Disposable Pattern
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _session?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
