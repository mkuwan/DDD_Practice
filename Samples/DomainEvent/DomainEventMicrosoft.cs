using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Samples.SeedWork;
using Xunit;

namespace Samples.DomainEvent
{
    public class DomainEventByMediatR
    {
        [Fact]
        public async void DomainEventByMediatRTest()
        {
            // Arrange
            Task actual = null;
            var mock = new Mock<IMediator>(); 
            mock
                .Setup(m => m.Publish(It.IsAny<INotification>(), default))
                .Returns(actual = Task.CompletedTask)
                .Verifiable();
            var entity = new UserEntity("123", "Suzuki", mock.Object);
            
            // Act
            entity.ChangedUserName("Toyota");
            entity.ChangedUserName("Nissan");
            await entity.Update(); // ここではEntityからドメインイベント発行をしていますが、DbContextの保存処理直前に行うことが好ましいです

            // Assertion
            Assert.Equal(Task.CompletedTask, actual);
            mock.Verify(x => x.Publish(It.IsAny<INotification>(), default), Times.Exactly(2));
        }
    }


    public class UserEntity : Entity
    {
        private readonly IMediator _mediator;


        public UserEntity(string userId, string userName, IMediator mediator) 
        {
            UserId = userId;
            UserName = userName;
            _mediator = mediator;
            //_mediator = new NoMediator();
        }

        public string UserId { get; init; }
        public string UserName { get; private set; }


        public void ChangedUserName(string newUserName)
        {
            this.UserName = newUserName;

            UserStatusChangedDomainEvent user = new UserStatusChangedDomainEvent(this.UserId, this.UserName);
            AddDomainEvent(user);
        }

        public async Task Update()
        {
            await _mediator.EntityDispatchDomainEventAsync(this);
        }
    }

    public class NoMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult<TResponse>(default(TResponse));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(object));
        }
    }

    //ドメイン イベント パブ/サブは MediatR を利用して非同期で実装される

    //イベントを生成し、ディスパッチする遅延アプローチ
    // ドメイン イベントをコレクションに追加した後、トランザクションを (EF の SaveChanges で) コミットする "直前" または "直後" に、
    // それらのドメイン イベントをディスパッチすることです
    // (このアプローチについては、Jimmy Bogard の「A better domain events pattern」(よりよいドメイン イベント パターン) を参照)。
    // https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/


    /// <summary>
    /// https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation#implement-domain-events
    /// ドメイン イベントの実装
    /// C# のドメイン イベントは、ドメインで発生したことに関連するすべての情報を含む、DTO のようなデータ保持構造体またはクラスです
    /// 
    /// ユビキタス言語の観点からは、イベントは過去に発生した事柄なので、
    /// イベントのクラス名は OrderStartedDomainEvent や OrderShippedDomainEvent のような過去形動詞として表される必要があります。
    /// これは、eShopOnContainers の注文マイクロサービスでのドメイン イベントの実装方法です
    ///
    /// イベントの重要な特性は、イベントは過去に発生したことなので変更できない、ということです。 したがって、不変クラスである必要があります
    /// </summary>
    public class UserStatusChangedDomainEvent : INotification
    {
        public string UserId { get; }
        public string UserName { get; }
        
        public UserStatusChangedDomainEvent(string userId, string userName)
        {
            UserId = userId;
            UserName = userName;
        }
    }
}
