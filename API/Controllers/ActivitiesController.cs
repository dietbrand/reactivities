using Application.Activities.Commands;
using Application.Activities.Queries;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ActivitiesController() : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> GetActivities(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetActivityList.Query(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivity(string id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetActivityDetails.Query(id), cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateActivity(Activity activity, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new CreateActivity.Command(activity), cancellationToken);
    }

    [HttpPut]
    public async Task<ActionResult> EditActivity(Activity activity, CancellationToken cancellationToken)
    {
        await Mediator.Send(new EditActivity.Command(activity), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteActivity(string id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteActivity.Command(id), cancellationToken);
        return NoContent();
    }
}