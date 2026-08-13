using HospitalSaaS.Application.Common.Behaviors;
using HospitalSaaS.Application.Common.Interfaces;
using HospitalSaaS.Application.Common.Messaging;

namespace HospitalSaaS.Application.Tests;

public sealed class TransactionBehaviorTests
{
    [Fact]
    public async Task Command_Is_Executed_Inside_A_Transaction()
    {
        var unitOfWork = new RecordingUnitOfWork();
        var behavior = new TransactionBehavior<TestCommand, string>(unitOfWork);

        var response = await behavior.Handle(
            new TestCommand(),
            _ => Task.FromResult("created"),
            CancellationToken.None
        );

        Assert.Equal("created", response);
        Assert.Equal(1, unitOfWork.TransactionCount);
    }

    [Fact]
    public async Task Query_Does_Not_Open_A_Transaction()
    {
        var unitOfWork = new RecordingUnitOfWork();
        var behavior = new TransactionBehavior<TestQuery, string>(unitOfWork);

        var response = await behavior.Handle(
            new TestQuery(),
            _ => Task.FromResult("patient"),
            CancellationToken.None
        );

        Assert.Equal("patient", response);
        Assert.Equal(0, unitOfWork.TransactionCount);
    }

    private sealed record TestCommand : ICommand<string>;

    private sealed record TestQuery : IQuery<string>;

    private sealed class RecordingUnitOfWork : IUnitOfWork
    {
        public int TransactionCount { get; private set; }

        public async Task<T> ExecuteInTransactionAsync<T>(
            Func<CancellationToken, Task<T>> operation,
            CancellationToken cancellationToken
        )
        {
            TransactionCount++;
            return await operation(cancellationToken);
        }
    }
}
