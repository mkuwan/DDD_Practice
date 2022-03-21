using Microsoft.AspNetCore.Mvc;
using SampleConsole.Application.Queries;
using SampleConsole.Infrastructure;

namespace SampleConsole.Application.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly SampleDbContext _context;
    private readonly IMediator _mediator;

    public ScheduleController(ILogger<ScheduleController> logger, SampleDbContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context;
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

        return Ok(result);
    }

}