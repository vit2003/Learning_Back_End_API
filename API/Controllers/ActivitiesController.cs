using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> List()
        {
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> Details(Guid id)
        {
            return await Mediator.Send(new Details.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id, Delete.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }

        [HttpPost("{activityId}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid activityId)
        {
            return await Mediator.Send(new Attend.Command { ActivityId = activityId });
        }

        [HttpDelete("{activityId}/unattend")]
        public async Task<ActionResult<Unit>> Unattend(Guid activityId)
        {
            return await Mediator.Send(new Unattend.Command { ActivityId = activityId });
        }
    }
}
