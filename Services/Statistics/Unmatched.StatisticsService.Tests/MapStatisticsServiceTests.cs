namespace Unmatched.StatisticsService.Tests;

using Moq;

using Unmatched.StatisticsService.Domain.Models;
using Unmatched.StatisticsService.Domain.Repositories;
using Unmatched.StatisticsService.Domain.Services;

public class MapStatisticsServiceTests
{
    private readonly Mock<IMapStatsRepository> _mapStatsRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly MapStatisticsService _service;

    public MapStatisticsServiceTests()
    {
        _unitOfWork.Setup(u => u.MapStats).Returns(_mapStatsRepository.Object);
        _service = new MapStatisticsService(mapper: null!, unitOfWork: _unitOfWork.Object);
    }

    [Fact]
    public async Task UpdateImageAsync_ExistingMap_StoresImageAndSaves()
    {
        var mapId = Guid.NewGuid();
        var stats = new MapStats { MapId = mapId, Name = "Raptor Paddock" };
        _mapStatsRepository.Setup(r => r.GetAsync(mapId)).ReturnsAsync(stats);

        var updated = await _service.UpdateImageAsync(mapId, "raptor-paddock.png");

        Assert.Equal("raptor-paddock.png", updated!.ImageFileName);
        _mapStatsRepository.Verify(r => r.AddOrUpdateAsync(stats), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateImageAsync_UnknownMap_ReturnsNull()
    {
        var mapId = Guid.NewGuid();
        _mapStatsRepository.Setup(r => r.GetAsync(mapId)).ReturnsAsync((MapStats?)null);

        var updated = await _service.UpdateImageAsync(mapId, "whatever.png");

        Assert.Null(updated);
        _mapStatsRepository.Verify(r => r.AddOrUpdateAsync(It.IsAny<MapStats>()), Times.Never);
    }
}
