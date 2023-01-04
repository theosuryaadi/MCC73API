using API.Contexts;
using API.Repositories;
using API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Base;

[Route("api/[controller]")]
[ApiController]
public class BaseController<Repository, Entity, T> : ControllerBase 
    where Entity : class
    where Repository : IRepository<Entity, T>
{
    private Repository _repo;

    public BaseController(Repository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public ActionResult GetAll()
    {
        try
        {
            var result = _repo.Get();
            return result.Count() == 0
            ? Ok(new { statusCode = 204, message = "Data Not Found!" })
            : Ok(new { statusCode = 201, message = "Success", data = result });
        }
        catch (Exception e)
        {
            return BadRequest(new { statusCode = 500, message = $"Something Wrong! : {e.Message}" });
        }

    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult GetById(T id)
    {
        try
        {
            var result = _repo.Get(id);
            return result == null
            ? Ok(new { statusCode = 204, message = $"Data With Id {id} Not Found!" })
            : Ok(new { statusCode = 201, message = $"Id {id}", data = result });
        }
        catch (Exception e)
        {
            return BadRequest(new { statusCode = 500, message = $"Something Wrong! : {e.Message}" });
        }
    }

    [HttpPost]
    public ActionResult Insert(Entity entity)
    {
        try
        {
            var result = _repo.Insert(entity);
            return result == null 
                ? Ok(new { statusCode = 204, message = "Data failed to Insert!" }) 
                : Ok(new { statusCode = 201, message = "Data Saved Succesfully!" });
        }
        catch
        {
            return BadRequest(new { statusCode = 500, message = "" });
        }
    }

    [HttpPut]
    public ActionResult Update(Entity entity)
    {
        try
        {
            var result = _repo.Update(entity);
            return result == null 
                ? Ok(new { statusCode = 204, message = $"Id {entity} not found!" })
                : Ok(new { statusCode = 201, message = "Update Succesfully!" });
        }
        catch
        {
            return BadRequest(new { statusCode = 500, message = "Something Wrong!" });
        }
    }

    [HttpDelete]
    public ActionResult Delete(T id)
    {
        try
        {
            var result = _repo.Delete(id);
            return result == null 
                ? Ok(new { statusCode = 204, message = $"Id {id} Data Not Found" }) 
                : Ok(new { statusCode = 201, message = "Data Delete Succesfully!" });
        }
        catch (Exception e)
        {
            return BadRequest(new { statusCode = 500, message = $"Something Wrong {e.Message}" });
        }
    }

}//end
