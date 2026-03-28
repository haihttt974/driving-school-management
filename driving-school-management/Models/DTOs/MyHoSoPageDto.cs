using System.Collections.Generic;

namespace driving_school_management.Models.DTOs
{
    public class MyHoSoPageDto
    {
        public List<MyHoSoCardDto> Cards { get; set; } = new List<MyHoSoCardDto>();
        public List<HoSoDetailDto> Details { get; set; } = new List<HoSoDetailDto>();
    }
}