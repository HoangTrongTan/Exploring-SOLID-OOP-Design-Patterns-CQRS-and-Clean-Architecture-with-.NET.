using AutoMapper;
using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Entities;

namespace CrudPractice.Application.Common.Mappings;

/// <summary>
/// [AutoMapper - Profile] Centralized mapping configuration.
/// Tách mapping logic khỏi handler → Single Responsibility.
///
/// [SOLID - Open/Closed] Thêm mapping mới → chỉ thêm vào đây,
/// không cần sửa handler hay controller.
///
/// Cách hoạt động:
///   _mapper.Map&lt;ProductDto&gt;(product) → AutoMapper tìm CreateMap&lt;Product, ProductDto&gt;
///   và tự map các property cùng tên.
///
/// [Interview term] "Object Mapping" hay "Object-to-Object Mapping"
/// AutoMapper alternatives: Mapster (faster), manual mapping (explicit).
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product → ProductDto
        // [TODO - BÀI TẬP 9] Thêm ForMember để map CategoryName từ Category?.Name
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Unknown"));

        // Product → ProductSummaryDto (lighter version for list views)
        CreateMap<Product, ProductSummaryDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Unknown"));

        // Category → CategoryDto
        CreateMap<Category, CategoryDto>();
    }
}
