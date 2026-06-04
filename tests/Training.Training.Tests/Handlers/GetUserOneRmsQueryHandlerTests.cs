using Moq;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;
using Training.Training.Handlers.OneRms.Queries;

namespace Training.Training.Tests.Handlers;

public class GetUserOneRmsQueryHandlerTests
{
    private readonly Mock<IOneRmRepository> _repo;
    private readonly GetUserOneRmsQueryHandler _handler;

    public GetUserOneRmsQueryHandlerTests()
    {
        _repo = new Mock<IOneRmRepository>();
        _handler = new GetUserOneRmsQueryHandler(_repo.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOneRms()
    {
        var oneRms = new List<OneRm>
        {
            OneRm.Hydrate(1, "user-1", 1, 100),
            OneRm.Hydrate(2, "user-1", 2, 150)
        };
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync(oneRms);

        var result = await _handler.Handle(new GetUserOneRmsQuery("user-1"), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WithNoData_ShouldReturnEmpty()
    {
        _repo.Setup(r => r.GetByUserIdAsync("user-1", It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await _handler.Handle(new GetUserOneRmsQuery("user-1"), CancellationToken.None);

        Assert.Empty(result);
    }
}
