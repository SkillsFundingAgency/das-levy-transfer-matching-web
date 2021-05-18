using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Client;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.LevyTransferMatching.Infrastructure.Services.AccountUsersReadStore;
using EmployerAccountsApiClient = SFA.DAS.LevyTransferMatching.Infrastructure.Api.EmployerAccountsApiClient;

namespace SFA.DAS.LevyTransferMatching.Infrastructure.UnitTests.Api
{
    [TestFixture]
    public class EmployerAccountsApiClientTests
    {
        private EmployerAccountsApiClient _employerAccountsApiClient;
        private Mock<IAccountUsersReadOnlyRepository> _mockAccountUsersReadOnlyRepository;
        private readonly Guid _userRef = Guid.NewGuid();

        [SetUp]
        public void Setup()
        {
            _mockAccountUsersReadOnlyRepository = new Mock<IAccountUsersReadOnlyRepository>();

            var accountUsers = new List<Services.AccountUsersReadStore.Types.AccountUser>();
            accountUsers.Add(new Services.AccountUsersReadStore.Types.AccountUser
            {
                accountId = 123, ETag = "", Id = Guid.NewGuid(), role = UserRole.Owner, removed = default , userRef = _userRef
            });

            var documentQuery = new FakeDocumentQuery<Services.AccountUsersReadStore.Types.AccountUser>(accountUsers);

            _mockAccountUsersReadOnlyRepository.Setup(m =>
                    m.CreateQuery(It.IsAny<FeedOptions>()))
                .Returns(documentQuery);

            _employerAccountsApiClient = new EmployerAccountsApiClient(_mockAccountUsersReadOnlyRepository.Object);
        }

        [Test]
        public async Task IsUserInRole_Returns_True_If_User_Is_In_Specified_Role()
        {
            var request = new IsUserInRoleRequest
                {AccountId = 123, UserRef = _userRef, Roles = new HashSet<UserRole> {UserRole.Owner}};

            var result = await _employerAccountsApiClient.IsUserInRole(request,CancellationToken.None);

            Assert.IsTrue(result);
        }

#pragma warning disable S3881 // "IDisposable" should be implemented correctly
        private class FakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T>
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
        {
            public Expression Expression => _query.Expression;
            public Type ElementType => _query.ElementType;
            public bool HasMoreResults => ++_page <= _pages;
            public IQueryProvider Provider => new FakeDocumentQueryProvider<T>(_query.Provider);

            private readonly IQueryable<T> _query;
            private readonly int _pages = 1;
            private int _page = 0;

            public FakeDocumentQuery(IEnumerable<T> data)
            {
                _query = data.AsQueryable();
            }

            public Task<FeedResponse<TResult>> ExecuteNextAsync<TResult>(CancellationToken token = default)
            {
                return Task.FromResult(new FeedResponse<TResult>(this.Cast<TResult>()));
            }

            public Task<FeedResponse<dynamic>> ExecuteNextAsync(CancellationToken token = default)
            {
                return Task.FromResult(new FeedResponse<dynamic>(this.Cast<dynamic>()));
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _query.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

#pragma warning disable S1186 // Methods should not be empty
            public void Dispose()
#pragma warning restore S1186 // Methods should not be empty
            {
            }
        }

        private class FakeDocumentQueryProvider<T> : IQueryProvider
        {
            private readonly IQueryProvider _queryProvider;

            public FakeDocumentQueryProvider(IQueryProvider queryProvider)
            {
                _queryProvider = queryProvider;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new FakeDocumentQuery<T>(_queryProvider.CreateQuery<T>(expression));
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new FakeDocumentQuery<TElement>(_queryProvider.CreateQuery<TElement>(expression));
            }

            public object Execute(Expression expression)
            {
                return _queryProvider.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _queryProvider.Execute<TResult>(expression);
            }
        }

    }
}
