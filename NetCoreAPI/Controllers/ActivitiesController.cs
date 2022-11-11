using Application.Activities;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers
{
    public class ActivitiesController : BaseApiController<Activity>
    {
        public ActivitiesController(IMediator mediator, IValidator<Activity> validator) : base(mediator, validator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await Mediator.Send(new ListOfActivities.Query());
            return HandleResult(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActivityById(Guid id)
        {
            var activity = await Mediator.Send(new Details.Query {Id = id});
            return HandleResult(activity);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] Activity activity) 
        {
            var validationResult = await _validator.ValidateAsync(activity);
            if (!validationResult.IsValid) return BadRequest(new { errors = validationResult.Errors });

            var result = await Mediator.Send(new Create.Command { Activity = activity });
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, [FromBody] Activity activity) 
        {
            var validationResult = await _validator.ValidateAsync(activity);
            if (!validationResult.IsValid) return BadRequest(new { errors = validationResult.Errors });

            activity.Id = id;
            var result = await Mediator.Send(new Edit.Command { Activity = activity });
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(Guid id) 
        {
            var result = await Mediator.Send(new Delete.Command { Id = id });
            return HandleResult(result);
        }
    }
}