using AutoMapper;
using ChatApp.DTOs.UserDTOs;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class Mapping:Profile
    {
        public Mapping()
        {
            CreateMap<RegisterRequestDTO, User>();
            CreateMap<User, RegisterResponseDTO>();
        }
    }
}
