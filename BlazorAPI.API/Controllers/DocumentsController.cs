using Microsoft.AspNetCore.Mvc;
using BlazorAPI.API.DTOs.DocumentDTO;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Services.Interface;

namespace BlazorAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
        {
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DocumentMetadataDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DocumentMetadataDto>>> GetAllMetadata(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve all document metadata");
            var documents = await _documentService.GetAllMetadataAsync(cancellationToken);
            return Ok(documents);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DocumentMetadataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentMetadataDto>> GetMetadataById(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve document metadata by ID: {DocumentId}", id);
            try
            {
                var document = await _documentService.GetMetadataByIdAsync(id, cancellationToken);
                return Ok(document);
            }
            catch (DocumentValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DocumentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}/full")]
        [ProducesResponseType(typeof(DocumentFullDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentFullDto>> GetFullDocumentById(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve full document (with content) by ID: {DocumentId}", id);
            try
            {
                var document = await _documentService.GetFullDocumentByIdAsync(id, cancellationToken);
                return Ok(document);
            }
            catch (DocumentValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DocumentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(DocumentMetadataDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DocumentMetadataDto>> CreateDocument([FromBody] DocumentCreateDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST request: Create a new document");
            try
            {
                var createdDocument = await _documentService.CreateDocumentAsync(dto, cancellationToken);
                return CreatedAtAction(nameof(GetMetadataById), new { id = createdDocument.Id }, createdDocument);
            }
            catch (DocumentValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateDocument(int id, [FromBody] DocumentUpdateDto dto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT request: Update document with ID: {DocumentId}", id);

            if (id != dto.Id)
            {
                return BadRequest(new { message = "The ID in the URL route does not match the ID in the document body." });
            }

            try
            {
                await _documentService.UpdateDocumentAsync(dto, cancellationToken);
                return NoContent();
            }
            catch (DocumentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DocumentValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDocument(int id, [FromQuery] int modifiedById, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE request: Delete document with ID: {DocumentId}", id);
            try
            {
                await _documentService.DeleteDocumentAsync(id, modifiedById, cancellationToken);
                return NoContent();
            }
            catch (DocumentNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (DocumentValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}