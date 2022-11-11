using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController<T> : ControllerBase
    {
        private readonly IMediator _mediator;
        protected readonly IValidator<T> _validator;
        protected IMediator Mediator => _mediator ?? HttpContext.RequestServices.GetService<IMediator>();

        public BaseApiController(IMediator mediator, IValidator<T> validator)
        {
            _mediator = mediator;
            _validator = validator;  
        }

        protected ActionResult HandleResult<U>(Result<U> result) {
            if(result == null) return NotFound();
            
            if (result.IsSuccess && result.Value != null)
                return Ok(result.Value);
            else if (result.IsSuccess && result.Value == null)
                return NotFound();
            else
                return BadRequest(result.Error);
        }
    }
}