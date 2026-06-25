using BlazorAPI.BlazorUI.DTOs.UserDTOs;

namespace BlazorAPI.BlazorUI.Services.Interface
{
    public interface iUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO?> GetById(int id);
        Task<UserDTO?> GetByEmail(string email);
        Task<UserDTO> CreateUser(CreateUserDTO createDTO);
        Task UpdateUser(int id, UpdateUserDTO updateDTO);
        Task DeleteUser(int id, int modifiedById);
        Task<UserDTO?> Login(LoginDTO loginDTO);
    }
}
