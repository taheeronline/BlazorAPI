using MovieManager.BlazorUI.DTOs.UserDTOs;

namespace MovieManager.BlazorUI.Services.Interface
{
    public interface iUserService
    {
        Task<IEnumerable<UserDTO>> GetAll();
        Task<UserDTO?> GetById(int id);
        Task<UserDTO?> GetByEmail(string email);
        Task<UserDTO> CreateUser(CreateUserDTO createDTO);
        Task UpdateUser(int id, UpdateUserDTO updateDTO);
        Task DeleteUser(int id);
        Task<UserDTO?> Login(LoginDTO loginDTO);
    }
}
