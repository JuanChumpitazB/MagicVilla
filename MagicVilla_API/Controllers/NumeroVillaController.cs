using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.DTO;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numVillaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;
        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo, INumeroVillaRepositorio numVillaRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _numVillaRepo = numVillaRepo;
            _mapper = mapper;
            _apiResponse = new APIResponse();
        }

        [HttpGet]
        [Route("GetNumeroVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las Villas");

                IEnumerable<NumeroVilla> lstNumeroVilla = await _numVillaRepo.ObtenerTodos();

                _apiResponse.Resultado = _mapper.Map<IEnumerable<NumeroVillaDTO>>(lstNumeroVilla);
                _apiResponse.statusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMenssages = new List<string>() { ex.ToString() };
            }

            return Ok(_apiResponse);
        }
        [HttpGet]
        [Route("GetNumeroVilla/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsExitoso = false;
                    _logger.LogError("Error al traer Villa con el Id " + id);
                    _apiResponse.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var numeroVilla = await _numVillaRepo.Obtener(x => x.VillaNo == id);
                if (numeroVilla == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse); 
                }

                _apiResponse.Resultado = _mapper.Map<NumeroVillaDTO>(numeroVilla);
                _apiResponse.statusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMenssages = new List<string>() { ex.ToString() };
                throw;
            }

            return _apiResponse;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDTO numeroCreateDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _numVillaRepo.Obtener(x => x.VillaNo == numeroCreateDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("NombreExiste", "El numero de Villa ya existe.");
                    return BadRequest(ModelState);
                }

                if(await _villaRepo.Obtener(x => x.Id == numeroCreateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El id de la Villa no existe.");
                    return BadRequest(ModelState);
                }

                if (numeroCreateDTO == null) return BadRequest(numeroCreateDTO);

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(numeroCreateDTO);
                modelo.FechaCreacion = DateTime.Now; 
                modelo.FechaActualizacion = DateTime.Now; 

                await _numVillaRepo.Crear(modelo);
                _apiResponse.Resultado = modelo;
                _apiResponse.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumeroVillas", new { id = modelo.VillaNo }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMenssages = new List<string>() { ex.ToString()};
                throw;
            }
            return _apiResponse;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsExitoso=false;
                    _apiResponse.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var numeroVilla = await _numVillaRepo.Obtener(x => x.VillaNo == id);
                if (numeroVilla == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _numVillaRepo.Remover(numeroVilla);

                _apiResponse.statusCode = HttpStatusCode.NoContent;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMenssages = new List<string>() { ex.ToString() };
                throw;
            }
            return BadRequest(_apiResponse);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDTO numUpdateDTO)
        {
            try
            {
                if (numUpdateDTO == null || id != numUpdateDTO.VillaNo)
                {
                    _apiResponse.IsExitoso=false;
                    _apiResponse.statusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                if (await _villaRepo.Obtener(x => x.Id == numUpdateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El id de la Villa no existe.");
                    return BadRequest(ModelState);
                }

                if (numUpdateDTO == null) return BadRequest();

                NumeroVilla modelo = _mapper.Map<NumeroVilla>(numUpdateDTO);
                modelo.FechaActualizacion = DateTime.Now;

                await _numVillaRepo.Actualizar(modelo);

                _apiResponse.statusCode = HttpStatusCode.NoContent;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsExitoso = false;
                _apiResponse.ErrorMenssages = new List<string>() { ex.ToString() };
                throw;
            }
            return BadRequest(_apiResponse);
        }

        
    }
}
