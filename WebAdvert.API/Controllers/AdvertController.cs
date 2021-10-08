using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAdvert.API.Services;

namespace WebAdvert.API.Controllers
{
    [ApiController]
    [Route("api/v1/adverts")]
    public class AdvertController : ControllerBase
    {
        private readonly ILogger<AdvertController> _logger;
        private readonly IAdvertStorageService _advertStorageService;
        public AdvertController(ILogger<AdvertController> logger, IAdvertStorageService advertStorageService)
        {
            _logger = logger;
            _advertStorageService = advertStorageService;
        }

        [HttpPost("create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(201, Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> CreateAdvert(AdvertModel model)
        {
            string recordId;
            try
            {
                recordId = await _advertStorageService.Add(model);                
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            var response = new CreateAdvertResponse()
            {
                Id = recordId
            };
            return StatusCode(StatusCodes.Status201Created, response);
        }

        [HttpPut("confirm")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertStorageService.Confirm(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok();
        }
    }
}
