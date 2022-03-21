using Microsoft.AspNetCore.Mvc;
using SampleConsole.Application.Commands;
using SampleConsole.Application.Queries;
using SampleConsole.Domain.SampleModel;
using SampleConsole.Infrastructure;

namespace SampleConsole.Application.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IMediator _mediator;

    public ScheduleController(ILogger<ScheduleController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation("HttpGet ");

        var query = new GetScheduleQuery();
        var result = await _mediator.Send(query);

        return Ok(result);

    }

    [HttpGet("{scheduleId}")]
    public async Task<IActionResult> GetSchedule(int scheduleId)
    {
        _logger.LogDebug($"Get by {scheduleId}");

        var query = new GetScheduleByIdQuery(scheduleId);
        var result = await _mediator.Send(query);

        //return result !=null ? Ok(result) : NotFound(); // こちらだとNotFound(404errorがでる)
        return Ok(result);  // こちらだとない場合は 204 No Contentとなる
    }

    [HttpPost]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleCommand command)
    {
        var result = await _mediator.Send(command);

        //return Ok(result);
        return CreatedAtAction(nameof(GetSchedule), new { scheduleId = result.Id }, result);
    }

}