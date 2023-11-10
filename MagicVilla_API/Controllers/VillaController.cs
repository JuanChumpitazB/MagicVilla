using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.DTO;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Obtener las Villas");

            IEnumerable<Villa> lstVilla = await _villaRepo.ObtenerTodos();

            return Ok(_mapper.Map<IEnumerable<VillaDTO>>(lstVilla));
        }
        [HttpGet]
        [Route("GetVilla/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer Villa con el Id " + id);
                return BadRequest();
            }
            var villa = await _villaRepo.Obtener(x => x.Id == id);
            if (villa == null) { return NotFound(); }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CrearVilla([FromBody] VillaCreateDTO createDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _villaRepo.Obtener(x => x.Nombre.ToLower() == createDTO.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre ya existe.");
                return BadRequest(ModelState);
            }
            if (createDTO == null) return BadRequest(createDTO);

            Villa modelo = _mapper.Map<Villa>(createDTO);

            await _villaRepo.Crear(modelo);

            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0) return BadRequest();
            var villa = await _villaRepo.Obtener(x => x.Id == id);
            if (villa == null) return NotFound();
            _villaRepo.Remover(villa);
            return NoContent();
        }
        
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id) return BadRequest();

            if(updateDTO == null) return BadRequest();

            Villa modelo = _mapper.Map<Villa>(updateDTO);

            _villaRepo.Actualizar(modelo);

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0) return BadRequest();

            var villa = await _villaRepo.Obtener(x => x.Id == id, tracked:false);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null) return BadRequest();

            patchDTO.ApplyTo(villaDTO, ModelState);

            if(!ModelState.IsValid) return BadRequest(ModelState);

            Villa modelo = _mapper.Map<Villa>(villaDTO);

            _villaRepo.Actualizar(modelo);

            return NoContent();
        }
    }
}
