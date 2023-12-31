﻿using AutoMapper;
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
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper;
        protected APIResponse _apiResponse;
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _apiResponse = new APIResponse();
        }

        [HttpGet]
        [Route("GetVillas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener las Villas");

                IEnumerable<Villa> lstVilla = await _villaRepo.ObtenerTodos();

                _apiResponse.Resultado = _mapper.Map<IEnumerable<VillaDTO>>(lstVilla);
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
        [Route("GetVilla/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
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
                var villa = await _villaRepo.Obtener(x => x.Id == id);
                if (villa == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse); 
                }

                _apiResponse.Resultado = _mapper.Map<VillaDTO>(villa);
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
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _villaRepo.Obtener(x => x.Nombre.ToLower() == createDTO.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("NombreExiste", "La Villa con ese nombre ya existe.");
                    return BadRequest(ModelState);
                }
                if (createDTO == null) return BadRequest(createDTO);

                Villa modelo = _mapper.Map<Villa>(createDTO);
                modelo.FechaCreacion = DateTime.Now; 
                modelo.FechaActualizacion = DateTime.Now; 

                await _villaRepo.Crear(modelo);
                _apiResponse.Resultado = modelo;
                _apiResponse.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _apiResponse);
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
        public async Task<IActionResult> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsExitoso=false;
                    _apiResponse.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }
                var villa = await _villaRepo.Obtener(x => x.Id == id);
                if (villa == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _villaRepo.Remover(villa);

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
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _apiResponse.IsExitoso=false;
                    _apiResponse.statusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                if (updateDTO == null) return BadRequest();

                Villa modelo = _mapper.Map<Villa>(updateDTO);
                modelo.FechaActualizacion = DateTime.Now;

                await _villaRepo.Actualizar(modelo);

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

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            try
            {
                if (patchDTO == null || id == 0) 
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: false);

                VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

                if (villa == null)
                {
                    _apiResponse.IsExitoso = false;
                    _apiResponse.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                patchDTO.ApplyTo(villaDTO, ModelState);

                if (!ModelState.IsValid) return BadRequest(ModelState);

                Villa modelo = _mapper.Map<Villa>(villaDTO);
                modelo.FechaActualizacion = DateTime.Now;

                await _villaRepo.Actualizar(modelo);

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
