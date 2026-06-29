using BlazorAPI.API.DTOs.UserDTOs;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Interface

{
    public interface IUserService
    {
        Task<PagedResult<UserDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<UserDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserDTO> CreateUserAsync(CreateUserDTO createDTO, CancellationToken cancellationToken = default);
        Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateDTO, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(int id, int modifiedById, CancellationToken cancellationToken = default);
        Task<UserDTO?> LoginAsync(LoginDTO loginDTO, CancellationToken cancellationToken = default);
    }
}
