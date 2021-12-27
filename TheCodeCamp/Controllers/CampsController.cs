using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
   [RoutePrefix("api/camps")]
  public class CampsController : ApiController
  {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository , IMapper mapper)
        {
            _repository= repository;
            _mapper = mapper;
        }
        [Route("{moniker}", Name = "GetCamp")]
        public async Task<IHttpActionResult> Get()
    {
            try
            {
                var result = await _repository.GetAllCampsAsync();

                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);
                
                return Ok(mappedResult);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
    }
        
        public async Task<IHttpActionResult> Post(CampModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var camp = _mapper.Map<Camp>(model);
                    _repository.AddCamp(camp);
                    if(await _repository.SaveChangesAsync())
                    {
                        var newModel = _mapper.Map<CampModel>(camp);
                        //var location = Url.Link();
                        return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker }, camp);
                    }
                }
                
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest();
        }
        [Route("{moniker}")]
        public async Task<IHttpActionResult> Put(string moniker, CampModel model)
        {
            try
            {
                var camp = _repository.GetCampAsync(moniker);
                if(camp!=null)
                {
                    return NotFound();
                }
                _mapper.Map(source: model, destination: camp);
                if(await _repository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<CampModel>(camp));
                }
                else
                {
                    return InternalServerError();
                }

            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();
                _repository.DeleteCamp(camp);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



    }
}
