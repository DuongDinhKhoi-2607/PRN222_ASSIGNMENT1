using BussinessLayer.DTOs;
using System.Threading.Tasks;

namespace BussinessLayer.Interfaces
{
    public interface IMessageCitationService
    {
        Task<MessageCitationDto> AddAsync(MessageCitationDto dto);
    }
}
