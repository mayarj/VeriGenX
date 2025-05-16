using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace VeriGenX.Infrastructure.Context
{
    public interface IMongoDbContext : IDisposable
    {
        IMongoCollection<T> GetCollection<T>(string name = null);

        #region Transaction Support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task AbortTransactionAsync();
        bool IsInTransaction { get; }
        #endregion

        #region Index Management
        Task CreateIndexAsync<T>(string field, CreateIndexOptions options = null, string collectionName = null);
        Task CreateUniqueIndexAsync<T>(string field, string collectionName = null);
        Task CreateCompoundIndexAsync<T>(IEnumerable<string> fields, CreateIndexOptions options = null, string collectionName = null);
        #endregion

        #region Helper Methods
        Task<bool> CollectionExistsAsync<T>(string collectionName = null);
        Task DropCollectionAsync<T>(string collectionName = null);
        Task<long> CountDocumentsAsync<T>(FilterDefinition<T> filter = null, string collectionName = null);
        Task<bool> AnyAsync<T>(FilterDefinition<T> filter = null, string collectionName = null);
        #endregion

        #region Bulk Operations
        Task BulkInsertAsync<T>(IEnumerable<T> documents, string collectionName = null);
        Task BulkUpdateAsync<T>(IEnumerable<WriteModel<T>> operations, string collectionName = null, BulkWriteOptions options = null);
        #endregion
    }
}
