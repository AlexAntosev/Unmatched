namespace Unmatched.CatalogService.Domain.Services;

using AutoMapper;

using Unmatched.CatalogService.Contracts.Kafka.Events;
using Unmatched.CatalogService.Domain.Entities;
using Unmatched.CatalogService.Domain.Repositories;

public class PlayStyleService(IUnitOfWork unitOfWork, IMapper mapper, IKafkaProducer producer) : IPlayStyleService
{
    public async Task<PlayStyle?> AddOrUpdateAsync(PlayStyle playStyle)
    {
        await unitOfWork.PlayStyles.AddOrUpdateAsync(playStyle);
        var addedEntity = await unitOfWork.PlayStyles.GetByIdAsync(playStyle.Id);

        await producer.PublishAsync("playstyle-updated", mapper.Map<PlayStyleUpdated>(addedEntity));

        return addedEntity;
    }

    public async Task<IEnumerable<PlayStyle>> GetAllAsync()
    {
        return await unitOfWork.PlayStyles.GetAsync();
    }

    public async Task<PlayStyle?> GetAsync(Guid heroId)
    {
        return await unitOfWork.PlayStyles.GetByHeroIdAsync(heroId);
    }
}