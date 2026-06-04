using MediatR;
using Training.Training.Domain.Entities;
using Training.Training.Domain.Repositories;

namespace Training.Training.Handlers.OneRms.Commands;

public class SaveOneRmsCommandHandler(IOneRmRepository repository) : IRequestHandler<SaveOneRmsCommand>
{
    public async Task Handle(SaveOneRmsCommand request, CancellationToken cancellationToken)
    {
        foreach (var (exerciseId, value) in request.Entries)
        {
            var oneRm = OneRm.Create(request.UserId, exerciseId, value);
            await repository.UpsertAsync(oneRm, cancellationToken);
        }
    }
}
