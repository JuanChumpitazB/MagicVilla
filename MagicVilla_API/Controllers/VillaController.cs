using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly AplicacionDBContext _dbContext;
        public VillaController(ILogger<VillaController> logger, AplicacionDBContext dBContext)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        [HttpGet]
        [Route("GetVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Obtener las Villas");
            return Ok(_dbContext.Villas.ToList());
        }
        [HttpGet]
        [Route("GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con el Id " + id);
                return BadRequest();
            }
            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null) { return NotFound(); }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CrearVilla([FromBody] VillaDTO villaDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (_dbContext.Villas.FirstOrDefault(x => x.Nombre.ToLower() == villaDTO.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre ya existe.");
                return BadRequest(ModelState);
            }
            if (villaDTO == null) return BadRequest(villaDTO);
            if (villaDTO.Id > 0) return StatusCode(StatusCodes.Status500InternalServerError);

            Villa villa = new()
            {
                Nombre = villaDTO.Nombre,
                Detalle = villaDTO.Detalle,
                ImagenURL = villaDTO.ImagenURL,
                Ocupantes = villaDTO.Ocupantes,
                MetrosCuadrados = villaDTO.MetrosCuadrados,
                Tarifa = villaDTO.Tarifa,
                Amenidad = villaDTO.Amenidad
            };
            _dbContext.Add(villa);
            _dbContext.SaveChanges();

            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0) return BadRequest();
            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);
            if (villa == null) return NotFound();
            _dbContext.Villas.Remove(villa);
            _dbContext.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id) return BadRequest();
            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);

            if(villa == null) return BadRequest();

            villa.Nombre = villaDTO.Nombre;
            villa.Detalle = villaDTO.Detalle;
            villa.ImagenURL = villaDTO.ImagenURL;
            villa.Ocupantes = villaDTO.Ocupantes;
            villa.MetrosCuadrados = villaDTO.MetrosCuadrados;
            villa.Tarifa = villaDTO.Tarifa;
            villa.Amenidad = villaDTO.Amenidad;
            _dbContext.Villas.Update(villa);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0) return BadRequest();

            var villa = _dbContext.Villas.FirstOrDefault(x => x.Id == id);

            VillaDTO villaDTO = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                Ocupantes = villa.Ocupantes,
                ImagenURL = villa.ImagenURL,
                MetrosCuadrados = villa.MetrosCuadrados,
                Tarifa = villa.Tarifa,
                Amenidad = villa.Amenidad
            };

            if (villa == null) return BadRequest();

            patchDTO.ApplyTo(villaDTO, ModelState);

            villa.Id = villaDTO.Id;
            villa.Nombre = villaDTO.Nombre;
            villa.Detalle = villaDTO.Detalle;
            villa.Ocupantes = villaDTO.Ocupantes;
            villa.ImagenURL = villaDTO.ImagenURL;
            villa.MetrosCuadrados = villaDTO.MetrosCuadrados;
            villa.Tarifa = villaDTO.Tarifa;
            villa.Amenidad = villaDTO.Amenidad;

            _dbContext.Villas.Update(villa);
            _dbContext.SaveChanges();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            return NoContent();
        }
    }
}
